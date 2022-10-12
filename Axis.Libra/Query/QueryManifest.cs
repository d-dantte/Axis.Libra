using Axis.Libra.URI;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Libra.Query
{
    /// <summary>
    /// Builder for the <see cref="QueryManifest"/>
    /// <para>
    /// NOTE: experiment with passing the <see cref="IRegistrarContract"/> as an argument into the <see cref="QueryManifestBuilder.AddQueryHandler{TQuery, TResult, TQueryHandler}(RegistryScope, InterceptorProfile)"/> method,
    /// rather than having it as a member field/property. This way, passing in the <see cref="IResolverContract"/> to the <see cref="QueryManifestBuilder.BuildManifest(IResolverContract)"/>
    /// method seems natural.
    /// </para>
    /// </summary>
    public class QueryManifestBuilder
    {
        private readonly IRegistrarContract _contract;
        private readonly Dictionary<Type, Type> _queryHandlerMap;
        private readonly HashSet<InstructionNamespace> _instructionNamespaces;

        public QueryManifestBuilder(IRegistrarContract contract)
        {
            _contract = contract
                ?.ThrowIf(c => c.IsRegistrationClosed(), new ArgumentException($"{nameof(contract)} is locked"))
                ?? throw new ArgumentNullException(nameof(contract));
            _queryHandlerMap = new Dictionary<Type, Type>();
            _instructionNamespaces = new HashSet<InstructionNamespace>();
        }

        /// <summary>
        /// Adds a query handler to the manifest, and also registers it with the underlying <see cref="IRegistrarContract"/>
        /// </summary>
        /// <typeparam name="TQuery">The query type</typeparam>
        /// <typeparam name="TQueryHandler">The handler type</typeparam>
        /// <param name="scope">The optional IoC registration scope</param>
        /// <param name="interceptorProfile">The optional interceptors for the query handler</param>
        /// <returns></returns>
        public QueryManifestBuilder AddQueryHandler<TQuery, TResult, TQueryHandler>(
            RegistryScope scope = default,
            InterceptorProfile interceptorProfile = default)
            where TQuery : IQuery<TResult>
            where TQueryHandler : class, IQueryHandler<TQuery, TResult>
        {
            ValidateHandlerMap(
                typeof(TQuery).ValuePair(typeof(TQueryHandler)),
                @namespace => _instructionNamespaces.Contains(@namespace));

            // placing this here so failed registrations will throw exceptions and subsequent statements wont execute
            _ = _contract.Register<TQueryHandler>(scope, interceptorProfile);
            _queryHandlerMap.Add(typeof(TQuery), typeof(TQueryHandler));
            _instructionNamespaces.Add(typeof(TQuery).InstructionNamespace());

            return this;
        }

        /// <summary>
        /// Creates a <see cref="QueryManifest"/> instance using the given resolver
        /// </summary>
        /// <param name="resolver">The service resolver</param>
        public QueryManifest BuildManifest(IResolverContract resolver)
        {
            return new QueryManifest(resolver, _queryHandlerMap);
        }


        /// <summary>
        /// check that the query map is valid:
        /// <list type="number">
        /// <item><see cref="IQuery{TResult}"/> is non-null</item>
        /// <item>query-type implements <see cref="IQuery{TResult}"/></item>
        /// <item>query-type is decorated with <see cref="InstructionNamespaceAttribute"/></item>
        /// <item><see cref="InstructionNamespaceAttribute"/> must be unique for each query</item>
        /// <item>handler-type implements <see cref="IQueryHandler{TQuery, TResult}"/>, where <c>TQuery</c> is query-type (key) in the <paramref name="queryMap"/>.</item>
        /// <item>handler type is non-null</item>
        /// <item>handler type implements <see cref="IQueryHandler{TQuery, TResult}"/></item>
        /// <item>handler must be a concrete class/struct</item>
        /// </list>
        /// </summary>
        /// <param name="commndMap">The map from query-type to query-handler-type</param>
        internal static void ValidateHandlerMap(
            KeyValuePair<Type, Type> queryMap,
            Func<InstructionNamespace, bool> containsNamespace)
        {
            var messagePrefix = "Invalid query-map:";

            if (containsNamespace == null)
                throw new ArgumentNullException(nameof(containsNamespace));

            #region query validation
            // query is non-null
            if (queryMap.Key is null)
                throw new InvalidOperationException($"{messagePrefix} query type cannot be null");

            // query-type implements IQuery
            if (!queryMap.Key.ImplementsGenericInterface(typeof(IQuery<>)))
                throw new InvalidOperationException($"{messagePrefix} query type does not implement {typeof(IQuery<>)}");

            // query-type is decorated with InstructionNamespaceAttribute
            if (!queryMap.Key.HasInstructionNamespace())
                throw new InvalidOperationException($"{messagePrefix} query type is not decorated with {nameof(InstructionNamespaceAttribute)}");

            // InstructionNamespaceAttribute must be unique for each query
            if (containsNamespace(queryMap.Key.InstructionNamespace()))
                throw new InvalidOperationException($"{messagePrefix} query type namespace '{queryMap.Key.InstructionNamespace()}' is not unique");
            #endregion

            #region query handler validation
            // handler-type is non-null
            if (queryMap.Value is null)
                throw new InvalidOperationException($"{messagePrefix} handler type cannot be null");

            // handler-type implements IQueryHandler<TQuery, TResult>
            var resultType = queryMap.Key.TryGetGenericInterface(typeof(IQuery<>), out var type)
                ? type.GetGenericArguments()[0]
                : throw new InvalidOperationException($"{messagePrefix} query type does not have a result type");
            var expectedInterface = typeof(IQueryHandler<,>).MakeGenericType(queryMap.Key, resultType);
            if (!queryMap.Value.Implements(expectedInterface))
                throw new InvalidOperationException($"{messagePrefix} handler type does not implement {expectedInterface}");

            // handler must be a concrete class/struct. No need to check for delegates or enums, as those types cannot implement interfaces
            if (!(!queryMap.Value.IsAbstract
                && !queryMap.Value.IsGenericTypeDefinition
                && !queryMap.Value.IsGenericParameter))
                throw new InvalidOperationException($"{messagePrefix} handler type must be a concrete type");
            #endregion
        }


        internal (Type queryType, Type queryHandlerType)[] QueryMaps() => _queryHandlerMap
            .Select(kvp => (kvp.Key, kvp.Value))
            .ToArray();
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of querys.
    /// It encapsulates a list of all <see cref="IQueryHandler{TQuery, TResult}"/> types, and corresponding <see cref="IQuery{TResult}"/> types in the system.
    /// </summary>
    public class QueryManifest
    {
        /// <summary>
        /// The service resolver
        /// </summary>
        private readonly IResolverContract _resolver;

        /// <summary>
        /// The namespace - query-type map
        /// </summary>
        private readonly Dictionary<InstructionNamespace, Type> _queryNamespaceMap = new Dictionary<InstructionNamespace, Type>();

        /// <summary>
        /// The query-type - query-handler-type map
        /// </summary>
        private readonly Dictionary<Type, Type> _queryHandlerMap;


        public QueryManifest(
            IResolverContract resolverContract,
            Dictionary<Type, Type> queryHandlerMap)
        {
            _resolver = resolverContract ?? throw new ArgumentNullException(nameof(resolverContract));
            _queryHandlerMap = queryHandlerMap
                .ThrowIfNull(new ArgumentNullException(nameof(queryHandlerMap)))
                .WithEach(kvp => QueryManifestBuilder.ValidateHandlerMap(kvp, _queryNamespaceMap.ContainsKey))
                .WithEach(kvp => _queryNamespaceMap.Add(kvp.Key.InstructionNamespace(), kvp.Key))
                .ToDictionary();
        }

        /// <summary>
        /// Gets all the query namespaces.
        /// </summary>
        public InstructionNamespace[] Namespaces() => _queryNamespaceMap.Keys.ToArray();

        /// <summary>
        /// Gets all the query types
        /// </summary>
        public Type[] QueryTypes() => _queryHandlerMap.Keys.ToArray();

        /// <summary>
        /// Gets the query type mapped to the given namespace, or null if no mapping exists.
        /// </summary>
        /// <param name="namespace">The namespace</param>
        public Type QueryTypeFor(InstructionNamespace @namespace)
            => _queryNamespaceMap.TryGetValue(@namespace, out var queryType)
                ? queryType
                : null;

        /// <summary>
        /// Gets the query handler mapped to the given <typeparamref name="TQuery"/>, or null if no mapping exists.
        /// </summary>
        /// <typeparam name="TQuery">The query type</typeparam>
        public IQueryHandler<TQuery, TResult> HandlerFor<TQuery, TResult>()
        where TQuery : IQuery<TResult>
        {
            return this
                .GetQueryHandlerTypeOrNull(typeof(TQuery))
                .AsOptional()
                .Map(type => _resolver.Resolve(type))
                .Map(Common.As<IQueryHandler<TQuery, TResult>>)
                .ValueOrDefault();
        }

        private Type GetQueryHandlerTypeOrNull(Type queryType)
            => _queryHandlerMap.TryGetValue(queryType, out var handlerType)
                    ? handlerType
                    : null;
    }
}
