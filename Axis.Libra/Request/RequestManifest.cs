using Axis.Libra.Instruction;
using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Libra.Request
{
    /// <summary>
    /// Builder for the <see cref="RequestManifest"/>
    /// </summary>
    public class RequestManifestBuilder
    {
        private readonly List<RequestInfo> _requestInfoList = new();
        private readonly HashSet<InstructionNamespace> _namespaces = new();
        private readonly HashSet<Type> _requestTypes = new();

        internal ImmutableArray<RequestInfo> RequestInfoList => _requestInfoList.ToImmutableArray();

        public RequestManifestBuilder()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TRequestHandler"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestHandler"></param>
        /// <param name="requestHasher"></param>
        /// <param name="namespace"></param>
        public RequestManifestBuilder AddRequestHandler<TRequest, TRequestHandler, TResult>(
            Func<TRequest, InstructionURI, TRequestHandler, TResult> requestHandler,
            Func<TRequest, ulong>? requestHasher = null,
            InstructionNamespace @namespace = default)
        {
            ArgumentNullException.ThrowIfNull(requestHandler);

            var _namespace = @namespace.IsDefault
                ? typeof(TRequest).DefaultInstructionNamespace("Request")
                : @namespace;

            Func<object, ulong> _hasher = requestHasher is not null
                ? reqObj => requestHasher.Invoke((TRequest)reqObj)
                : PropertySerializer.HashProperties;

            if (!_namespaces.Add(_namespace))
                throw new InvalidOperationException(
                    $"Invalid {nameof(@namespace)}: duplicate value '{@namespace}'");

            if (!_requestTypes.Add(typeof(TRequest)))
                throw new InvalidOperationException(
                    $"Invalid request type: duplicate value '{typeof(TRequest)}'");

            var info = new RequestInfo(
                _namespace,
                typeof(TRequest),
                typeof(TRequestHandler),
                (reqObj, uri, hnldObj) => requestHandler.Invoke((TRequest)reqObj, uri, (TRequestHandler)hnldObj)!,
                _hasher);
            _requestInfoList.Add(info);

            return this;
        }

        /// <summary>
        /// Creates a <see cref="RequestManifest"/> instanc
        /// </summary>
        public RequestManifest BuildManifest() => new(_requestInfoList);
    }

    /// <summary>
    /// A manifest is built out of the completed registration of requests.
    /// </summary>
    public class RequestManifest
    {
        /// <summary>
        /// The namespace - request-type map
        /// </summary>
        private readonly Dictionary<InstructionNamespace, RequestInfo> _requestNamespaceMap = new();

        /// <summary>
        /// The request-type - request-handler-type map
        /// </summary>
        private readonly Dictionary<Type, RequestInfo> _requestTypeMap = new();

        public RequestManifest(IEnumerable<RequestInfo> infoList)
        {
            infoList
                .ThrowIfNull(() => new ArgumentNullException(nameof(infoList)))
                .ThrowIfAny(
                    info => info.IsDefault,
                    _ => new ArgumentException($"Invalid {nameof(infoList)}: contains default"))
                .ForAll(info =>
                {
                    _requestNamespaceMap.Add(info.Namespace, info);
                    _requestTypeMap.Add(info.RequestType, info);
                });
        }

        /// <summary>
        /// Gets all the request namespaces.
        /// </summary>
        public ImmutableArray<InstructionNamespace> Namespaces() => _requestNamespaceMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets all the request types
        /// </summary>
        public ImmutableArray<Type> RequestTypes() => _requestTypeMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets the request type mapped to the given request type, or null if no mapping exists.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        internal RequestInfo? GetRequestInfo<TRequest>()
            => _requestTypeMap.TryGetValue(typeof(TRequest), out var requestInfo)
                ? requestInfo
                : null;

        /// <summary>
        /// Gets the request type mapped to the given namespace, or null if no mapping exists.
        /// </summary>
        /// <param name="namespace">The namespace</param>
        internal RequestInfo? GetRequestInfo(InstructionNamespace @namespace)
            => _requestNamespaceMap.TryGetValue(@namespace, out var requestInfo)
                ? requestInfo
                : null;
    }
}
