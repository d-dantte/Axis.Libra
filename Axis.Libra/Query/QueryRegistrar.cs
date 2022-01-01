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
    /// 
    /// Note that the query registrar ensures that the correct <see cref="Utils.BindQueryResultAttribute"/> bindings are respected during registration
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
        public QueryRegistrar AddHandlerRegistration<THandler, TQuery, TQueryResult>(
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        where TQuery : IQuery
        where TQueryResult : IQueryResult
        where THandler : class, IQueryHandler<TQuery, TQueryResult>
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            typeof(IQuery).ValidateQueryType();

            RegistrationsCache
                .GetOrAdd(typeof(IQueryHandler<TQuery, TQueryResult>), key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                .Add((typeof(THandler), scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Register a specific CommandResult Query handler type.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TCommandResult"></typeparam>
        /// <param name="scope"></param>
        /// <param name="interceptorProfile"></param>
        /// <returns></returns>
        public QueryRegistrar AddCommandResultHandlerRegistration<THandler, TCommandResult>(
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        where TCommandResult : ICommandResult
        where THandler : IQueryHandler<CommandResultQuery, TCommandResult>
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            RegistrationsCache
                .GetOrAdd(
                    typeof(IQueryHandler<CommandResultQuery, TCommandResult>),
                    key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                .Add((typeof(THandler), scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Register a specific query handler type
        /// </summary>
        /// <param name="queryHandlerType"></param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddHandlerRegistration(
            Type queryHandlerType,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            queryHandlerType
                .ThrowIfNull(new ArgumentNullException(nameof(queryHandlerType)))
                .ThrowIf(
                    t => !t.ImplementsGenericInterface(typeof(IQueryHandler<,>)),
                    new ArgumentException($"Type must implement {typeof(IQueryHandler<,>)}"))
                .GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .ForAll(@interface =>
                {
                    RegistrationsCache
                        .GetOrAdd(@interface, key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                        .Add((queryHandlerType, scope, interceptorProfile));
                });

            return this;
        }

        /// <summary>
        /// Register a specific CommandResult query handler type
        /// </summary>
        /// <param name="queryHandlerType">The type of the CommandResult Query Handler</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddCommandResultHandlerRegistration(
            Type queryHandlerType,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            queryHandlerType
                .ThrowIfNull(new ArgumentNullException(nameof(queryHandlerType)))
                .ThrowIf(
                    t => !t.ImplementsGenericInterface(typeof(IQueryHandler<,>)),
                    new ArgumentException($"Type must implement {typeof(IQueryHandler<,>)}"))
                .GetInterfaces()
                .Where(IsCommandResultQueryHandler)
                .ForAll(@interface =>
                {
                    RegistrationsCache
                        .GetOrAdd(@interface, key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                        .Add((queryHandlerType, scope, interceptorProfile));
                });

            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="IQueryHandler{TQuery}"/> found within the given namespace alone, for the assembly of the CALLING method
        /// </summary>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddNamespaceHandlerRegistrations(
            string @namespace,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            return AddNamespaceHandlerRegistrations(
                new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly,
                @namespace,
                scope,
                interceptorProfile);
        }

        /// <summary>
        /// Registers all instances of <see cref="IQueryHandler{TQuery}"/> found within the given namespace alone, for the supplied assembly.
        /// </summary>
        /// <param name="assembly">assembly to search within</param>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public QueryRegistrar AddNamespaceHandlerRegistrations(
            Assembly assembly,
            string @namespace,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            @namespace.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(@namespace)}"));

            assembly
                .GetExportedTypes()
                .Where(t => t.Namespace.Equals(@namespace))
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


        public QueryManifest BuildManifest()
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
                    IocRegistrar.Register(kvp.Key, registration.Item1, registration.Item2);

                //IocRegistrar.RegisterCollection(kvp.Key, registration.Item1, registration.Item2, kvp.Value);
            }
        }

        internal static bool IsCommandResultQueryHandler(Type interfaceType)
        {
            if (interfaceType == null)
                return false;

            if (interfaceType.IsGenericType)
                return false;

            if (interfaceType.GetGenericTypeDefinition() != typeof(IQueryHandler<,>))
                return false;

            var qtype = interfaceType.GetGenericArguments()[0];
            var qrtype = interfaceType.GetGenericArguments()[1];

            if (qtype != typeof(CommandResultQuery))
                return false;

            if (!qrtype.Implements(typeof(ICommandResult)))
                return false;

            return true;
        }
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of querys. It encapsulates a list of all <see cref="IQuery"/>s and corresponding <see cref="IQueryResult"/>s in the system.
    /// </summary>
    public class QueryManifest
    {
        public IEnumerable<QueryInfo> Queries { get; }

        public IEnumerable<Type> QueryHandlers { get; }

        public IEnumerable<Type> CommandResultHandlers => QueryHandlers.Where(QueryRegistrar.IsCommandResultQueryHandler);

        /// <summary>
        /// Creates a new instance of the <see cref="QueryManifest"/>
        /// </summary>
        /// <param name="types">List of <see cref="IQueryHandler{TQuery, TQueryResult}"/> interface types.</param>
        internal QueryManifest(IEnumerable<Type> types)
        {
            (QueryHandlers, Queries) = types.Aggregate((new List<Type>(), new List<QueryInfo>()), (lists, next) =>
            {
                lists.Item1.Add(next);

                var commandType = next
                    .GetGenericInterface(typeof(IQueryHandler<,>))
                    .GetGenericArguments()[0];
                var commandResutType = commandType
                    .GetAttribute<BindQueryResultAttribute>()
                    .ResultType;

                lists.Item2.Add(new QueryInfo(commandType, commandResutType));

                return lists;
            });
        }


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
