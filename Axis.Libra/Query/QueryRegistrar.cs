using Axis.Libra.Command;
using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Axis.Libra.Query
{
    /// <summary>
    /// Provides the service of discovering and registering querys and dispatchers with a supplied <see cref="Proteus.IoC.ServiceRegistrar"/>.
    /// </summary>
    public class QueryRegistrar
    {
        /// <summary>
        /// The underlying IoC registrar.
        /// </summary>
        private ServiceRegistrar IocRegistrar { get; }

        /// <summary>
        /// The registration cache. A dictionary of <see cref="IQueryHandler{TQuery, TQueryResult}"/> types mapped to their implementations and registration information
        /// </summary>
        private Dictionary<Type, HashSet<(Type, RegistryScope?, InterceptorProfile?)>> RegistrationsCache { get; } = new Dictionary<Type, HashSet<(Type, RegistryScope?, InterceptorProfile?)>>();

        /// <summary>
        /// The manifest. Once built, the <see cref="RegistrationsCache"/> is purged permanently
        /// </summary>
        private QueryManifest Manifest { get; set; }

        /// <summary>
        /// Return a new <see cref="QueryRegistrar"/> that is ready to register queries.
        /// </summary>
        /// <param name="iocRegistrar">The underlying IoC Registrar into which the instances are registered</param>
        public static QueryRegistrar BeginRegistration(ServiceRegistrar iocRegistrar) => new QueryRegistrar(iocRegistrar);


        private QueryRegistrar(ServiceRegistrar iocRegistrar)
        {
            IocRegistrar = iocRegistrar ?? throw new ArgumentNullException(nameof(iocRegistrar));
        }

        /// <summary>
        /// Register a specific query handler type.
        /// </summary>
        /// <typeparam name="THandler">The concrete handler implementation type</typeparam>
        /// <typeparam name="TQuery">The concrete command type</typeparam>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddHandlerRegistration<THandlerImplementation, TQuery, TQueryResult>(
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        where TQuery : IQuery
        where TQueryResult : IQueryResult
        where THandlerImplementation : class, IQueryHandler<TQuery, TQueryResult>
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            typeof(IQuery).ValidateQueryType();

            RegistrationsCache
                .GetOrAdd(typeof(IQueryHandler<TQuery, TQueryResult>), key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                .Add((typeof(THandlerImplementation), scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Register a specific query handler type
        /// </summary>
        /// <param name="queryHandlerImplementationType"></param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddHandlerRegistration(
            Type queryHandlerImplementationType,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            queryHandlerImplementationType
                .ValidateQueryHandlerImplementation()
                .GetInterfaces()
                .Where(@interface => @interface.IsGenericType)
                .Where(@interface => @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                // for each of the implemented query handlers, register the implementation type
                .ForAll(@interface =>
                {
                    RegistrationsCache
                        .GetOrAdd(@interface, key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                        .Add((queryHandlerImplementationType, scope, interceptorProfile));
                });

            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="IQueryHandler{TQuery}"/> found within the given namespace alone, for the assembly of the CALLING method
        /// </summary>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <param name="recursiveSearch">Indicates if recursive namespace search is required</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddNamespaceHandlerRegistrations(
            string @namespace,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null,
            bool recursiveSearch = false)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            return AddNamespaceHandlerRegistrations(
                new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly,
                @namespace,
                scope,
                interceptorProfile,
                recursiveSearch);
        }

        /// <summary>
        /// Registers all instances of <see cref="IQueryHandler{TQuery}"/> found within the given namespace alone, for the supplied assembly.
        /// </summary>
        /// <param name="assembly">assembly to search within</param>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <param name="recursiveSearch">Indicates if recursive namespace search is required</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddNamespaceHandlerRegistrations(
            Assembly assembly,
            string @namespace,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null,
            bool recursiveSearch = false)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            @namespace.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(@namespace)}"));

            assembly
                .GetExportedTypes()
                .Where(t => recursiveSearch? t.Namespace.IsChildNamespaceOf(@namespace): t.Namespace.Equals(@namespace, StringComparison.InvariantCulture))
                .Where(t => t.ImplementsGenericInterface(typeof(IQueryHandler<,>)))
                .ForAll(t => AddHandlerRegistration(t, scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="IQueryHandler{TQuery}"/> found within the supplied assembly.
        /// </summary>
        /// <param name="assembly">assembly to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddAssemblyHandlerRegistrations(
            Assembly assembly,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            assembly.ThrowIfNull(new ArgumentNullException(nameof(assembly)));

            assembly
                .GetExportedTypes()
                .Where(t => t.ImplementsGenericInterface(typeof(IQueryHandler<,>)))
                .ForAll(t => AddHandlerRegistration(t, scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Commits all the added registrations to the underlying IoC container, and builds a manifest for the commited registrations.
        /// </summary>
        /// <returns>the query manifest</returns>
        public QueryManifest CommitRegistrations()
        {
            if (Manifest == null)
            {
                FinalizeRegistrations();
                Manifest = new QueryManifest(RegistrationsCache.Keys);
                RegistrationsCache.Clear();
            }

            return Manifest;
        }

        private void FinalizeRegistrations()
        {
            foreach (var kvp in RegistrationsCache)
            {
                foreach (var registration in kvp.Value)
                    IocRegistrar.Register(
                        kvp.Key,
                        registration.Item1,
                        registration.Item2,
                        registration.Item3);
            }
        }
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of querys. It encapsulates a list of all <see cref="IQuery"/>s and corresponding <see cref="IQueryResult"/>s in the system.
    /// </summary>
    public class QueryManifest
    {
        /// <summary>
        /// A list of <see cref="QueryInfo"/> types representing all queries registered.
        /// </summary>
        public IEnumerable<QueryInfo> Queries { get; }

        /// <summary>
        /// A list of <see cref="Type"/> representing all of the <see cref="IQueryHandler{TQuery, TQueryResult}"/> interface types registered.
        /// </summary>
        public IEnumerable<Type> QueryHandlers { get; }

        /// <summary>
        /// A list of <see cref="Type"/> representing a subset of all <see cref="QueryManifest.QueryHandlers"/> which implement <see cref="IQueryHandler{TQuery, TQueryResult}"/>
        /// where <c>TQuery</c> is <see cref="CommandResultQuery"/>, and <c>TQueryResult</c> is <see cref="ICommandResult"/>.
        /// </summary>
        public IEnumerable<Type> CommandResultHandlers => QueryHandlers.Where(TypeExtensions.TryValidateCommandResultQueryHandlerImplementation);

        /// <summary>
        /// Creates a new instance of the <see cref="QueryManifest"/>
        /// </summary>
        /// <param name="types">List of <see cref="IQueryHandler{TQuery, TQueryResult}"/> interface types.</param>
        internal QueryManifest(IEnumerable<Type> types)
        {
            //what is going on here?
            (QueryHandlers, Queries) = types.Aggregate((new List<Type>(), new List<QueryInfo>()), (lists, next) =>
            {
                lists.Item1.Add(next);

                var genericArgs = next.GetGenericArguments();
                var queryType = genericArgs[0];
                var queryResutType = genericArgs[1];

                lists.Item2.Add(new QueryInfo(queryType, queryResutType));

                return lists;
            });
        }


        /// <summary>
        /// Struct that encapsulates the unique types that represent a <see cref="IQuery"/>
        /// </summary>
        public struct QueryInfo
        {
            public Type QueryType { get; }

            public Type QueryResultType { get; }

            public QueryInfo(Type queryType, Type queryResultType)
            {
                QueryType = queryType ?? throw new ArgumentNullException(nameof(queryType));
                QueryResultType = queryResultType ?? throw new ArgumentNullException(nameof(queryResultType));
            }

            public override int GetHashCode() => HashCode.Combine(QueryType, QueryResultType);

            public override bool Equals(object obj)
            {
                return obj is QueryInfo other
                    && other.QueryType == QueryType
                    && other.QueryResultType == QueryResultType;
            }
        }
    }
}
