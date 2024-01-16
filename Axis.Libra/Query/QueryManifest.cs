using Axis.Libra.Instruction;
using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Axis.Libra.Query
{
    /// <summary>
    /// Builder for the <see cref="QueryManifest"/>
    /// </summary>
    public class QueryManifestBuilder
    {
        private readonly List<QueryInfo> _queryInfoList = new();
        private readonly HashSet<InstructionNamespace> _namespaces = new();
        private readonly HashSet<Type> _queryTypes = new();

        internal ImmutableArray<QueryInfo> QueryInfoList => _queryInfoList.ToImmutableArray();

        public QueryManifestBuilder()
        { }

        /// <summary>
        /// Adds a query handler to the manifest, and also registers it with the underlying <see cref="IRegistrarContract"/>
        /// </summary>
        /// <typeparam name="TQuery">The query type</typeparam>
        /// <typeparam name="TQueryHandler">The handler type</typeparam>
        /// <param name="scope">The optional IoC registration scope</param>
        /// <param name="interceptorProfile">The optional interceptors for the query handler</param>
        /// <returns></returns>
        public QueryManifestBuilder AddQueryHandler<TQuery, TQueryHandler, TResult>(
            Func<TQuery, InstructionURI, TQueryHandler, Task<TResult>> queryHandler,
            Func<TQuery, ulong>? queryHasher = null,
            InstructionNamespace @namespace = default)
        {
            ArgumentNullException.ThrowIfNull(queryHandler);

            var _namespace = @namespace.IsDefault
                ? typeof(TQuery).DefaultInstructionNamespace("Query")
                : @namespace;

            Func<object, ulong> _hasher = queryHasher is not null
                ? qryObj => queryHasher.Invoke((TQuery)qryObj)
                : PropertySerializer.HashProperties;

            if (!_namespaces.Add(_namespace))
                throw new InvalidOperationException(
                    $"Invalid {nameof(@namespace)}: duplicate value '{@namespace}'");

            if (!_queryTypes.Add(typeof(TQuery)))
                throw new InvalidOperationException(
                    $"Invalid query type: duplicate value '{typeof(TQuery)}'");

            var info = new QueryInfo(
                _namespace,
                typeof(TQuery),
                typeof(TQueryHandler),
                (qryObj, uri, hnldObj) => queryHandler.Invoke((TQuery)qryObj, uri, (TQueryHandler)hnldObj),
                _hasher);
            _queryInfoList.Add(info);

            return this;
        }

        /// <summary>
        /// Creates a <see cref="QueryManifest"/> instance using the given resolver
        /// </summary>
        /// <param name="resolver">The service resolver</param>
        public QueryManifest BuildManifest() => new(_queryInfoList);
    }

    /// <summary>
    /// A manifest is built out of the complete registration of querys.
    /// </summary>
    public class QueryManifest
    {
        /// <summary>
        /// The namespace - query-type map
        /// </summary>
        private readonly Dictionary<InstructionNamespace, QueryInfo> _queryNamespaceMap = new();

        /// <summary>
        /// The query-type - query-handler-type map
        /// </summary>
        private readonly Dictionary<Type, QueryInfo> _queryTypeMap = new();

        public QueryManifest(IEnumerable<QueryInfo> infoList)
        {
            infoList
                .ThrowIfNull(() => new ArgumentNullException(nameof(infoList)))
                .ThrowIfAny(
                    info => info.IsDefault,
                    _ => new ArgumentException($"Invalid {nameof(infoList)}: contains default"))
                .ForAll(info =>
                {
                    _queryNamespaceMap.Add(info.Namespace, info);
                    _queryTypeMap.Add(info.QueryType, info);
                });
        }

        /// <summary>
        /// Gets all the query namespaces.
        /// </summary>
        public ImmutableArray<InstructionNamespace> Namespaces() => _queryNamespaceMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets all the query types
        /// </summary>
        public ImmutableArray<Type> QueryTypes() => _queryTypeMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets the query type mapped to the given query type, or null if no mapping exists.
        /// </summary>
        /// <typeparam name="TQuery">The query type</typeparam>
        internal QueryInfo? GetQueryInfo<TQuery>()
            => _queryTypeMap.TryGetValue(typeof(TQuery), out var queryInfo)
                ? queryInfo
                : null;

        /// <summary>
        /// Gets the query type mapped to the given namespace, or null if no mapping exists.
        /// </summary>
        /// <param name="namespace">The namespace</param>
        internal QueryInfo? GetQueryInfo(InstructionNamespace @namespace)
            => _queryNamespaceMap.TryGetValue(@namespace, out var queryInfo)
                ? queryInfo
                : null;
    }
}
