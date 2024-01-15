using Axis.Libra.Command;
using Axis.Libra.Instruction;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Libra.Request
{
    /// <summary>
    /// Builder for the <see cref="RequestManifest"/>
    /// <para>
    /// NOTE: experiment with passing the <see cref="IRegistrarContract"/> as an argument into the <see cref="RequestManifestBuilder.AddRequestHandler{TRequest, TResult, TRequestHandler}(RegistryScope, InterceptorProfile)"/> method,
    /// rather than having it as a member field/property. This way, passing in the <see cref="IResolverContract"/> to the <see cref="RequestManifestBuilder.BuildManifest(IResolverContract)"/>
    /// method seems natural.
    /// </para>
    /// </summary>
    public class RequestManifestBuilder
    {
        private readonly IRegistrarContract _contract;
        private readonly Dictionary<Type, Type> _requestHandlerMap;
        private readonly HashSet<InstructionNamespace> _instructionNamespaces;

        public RequestManifestBuilder(IRegistrarContract contract)
        {
            _contract = contract
                ?.ThrowIf(
                    c => c.IsRegistrationClosed(),
                    _ => new ArgumentException($"{nameof(contract)} is locked"))
                ?? throw new ArgumentNullException(nameof(contract));
            _requestHandlerMap = new Dictionary<Type, Type>();
            _instructionNamespaces = new HashSet<InstructionNamespace>();
        }

        /// <summary>
        /// Adds a request handler to the manifest, and also registers it with the underlying <see cref="IRegistrarContract"/>
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TRequestHandler">The handler type</typeparam>
        /// <param name="scope">The optional IoC registration scope</param>
        /// <param name="interceptorProfile">The optional interceptors for the request handler</param>
        /// <returns></returns>
        public RequestManifestBuilder AddRequestHandler<TRequest, TResult, TRequestHandler>(
            ResolutionScope scope = default,
            InterceptorProfile interceptorProfile = default)
            where TRequest : IRequest<TResult>
            where TRequestHandler : class, IRequestHandler<TRequest, TResult>
        {
            ValidateHandlerMap(
                typeof(TRequest).ValuePair(typeof(TRequestHandler)),
                @namespace => _instructionNamespaces.Contains(@namespace));

            // placing this here so failed registrations will throw exceptions and subsequent statements wont execute
            _ = _contract.Register<TRequestHandler>(scope, interceptorProfile);
            _requestHandlerMap.Add(typeof(TRequest), typeof(TRequestHandler));
            _instructionNamespaces.Add(typeof(TRequest).InstructionNamespace());

            return this;
        }

        /// <summary>
        /// Creates a <see cref="RequestManifest"/> instance using the given resolver
        /// </summary>
        /// <param name="resolver">The service resolver</param>
        public RequestManifest BuildManifest(IResolverContract resolver)
        {
            return new RequestManifest(resolver, _requestHandlerMap);
        }

        /// <summary>
        /// check that the request map is valid:
        /// <list type="number">
        /// <item><see cref="IRequest{TResult}"/> is non-null</item>
        /// <item>request-type implements <see cref="IRequest{TResult}"/></item>
        /// <item>request-type is decorated with <see cref="InstructionNamespaceAttribute"/></item>
        /// <item><see cref="InstructionNamespaceAttribute"/> must be unique for each request</item>
        /// <item>handler-type implements <see cref="IRequestHandler{TRequest, TResult}"/>, where <c>TRequest</c> is request-type (key) in the <paramref name="requestMap"/>.</item>
        /// <item>handler type is non-null</item>
        /// <item>handler type implements <see cref="IRequestHandler{TRequest, TResult}"/></item>
        /// <item>handler must be a concrete class/struct</item>
        /// </list>
        /// </summary>
        /// <param name="commndMap">The map from request-type to request-handler-type</param>
        internal static void ValidateHandlerMap(
            KeyValuePair<Type, Type> requestMap,
            Func<InstructionNamespace, bool> containsNamespace)
        {
            var messagePrefix = "Invalid request-map:";

            if (containsNamespace == null)
                throw new ArgumentNullException(nameof(containsNamespace));

            #region request validation
            // request is non-null
            if (requestMap.Key is null)
                throw new InvalidOperationException($"{messagePrefix} request type cannot be null");

            // request-type implements IRequest
            if (!requestMap.Key.ImplementsGenericInterface(typeof(IRequest<>)))
                throw new InvalidOperationException($"{messagePrefix} request type does not implement {typeof(IRequest<>)}");

            // InstructionNamespaceAttribute must be unique for each request
            if (containsNamespace(requestMap.Key.InstructionNamespace()))
                throw new InvalidOperationException($"{messagePrefix} request type namespace '{requestMap.Key.InstructionNamespace()}' is not unique");
            #endregion

            #region request handler validation
            // handler-type is non-null
            if (requestMap.Value is null)
                throw new InvalidOperationException($"{messagePrefix} handler type cannot be null");

            // handler-type implements IRequestHandler<TRequest, TResult>
            var resultType = requestMap.Key.TryGetGenericInterface(typeof(IRequest<>), out var type)
                ? type.GetGenericArguments()[0]
                : throw new InvalidOperationException($"{messagePrefix} request type does not have a result type");
            var expectedInterface = typeof(IRequestHandler<,>).MakeGenericType(requestMap.Key, resultType);
            if (!requestMap.Value.Implements(expectedInterface))
                throw new InvalidOperationException($"{messagePrefix} handler type does not implement {expectedInterface}");

            // handler must be a concrete class/struct. No need to check for delegates or enums, as those types cannot implement interfaces
            if (!(!requestMap.Value.IsAbstract
                && !requestMap.Value.IsGenericTypeDefinition
                && !requestMap.Value.IsGenericParameter))
                throw new InvalidOperationException($"{messagePrefix} handler type must be a concrete type");
            #endregion
        }

        internal (Type queryType, Type queryHandlerType)[] RequestMaps() => _requestHandlerMap
            .Select(kvp => (kvp.Key, kvp.Value))
            .ToArray();
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of requests.
    /// It encapsulates a list of all <see cref="IRequestHandler{TRequest, TResult}"/> types, and corresponding <see cref="IRequest{TResult}"/> types in the system.
    /// </summary>
    public class RequestManifest
    {
        /// <summary>
        /// The service resolver
        /// </summary>
        private readonly IResolverContract _resolver;

        /// <summary>
        /// The namespace - request-type map
        /// </summary>
        private readonly Dictionary<InstructionNamespace, Type> _requestNamespaceMap = new Dictionary<InstructionNamespace, Type>();

        /// <summary>
        /// The request-type - request-handler-type map
        /// </summary>
        private readonly Dictionary<Type, Type> _requestHandlerMap;


        public RequestManifest(
            IResolverContract resolverContract,
            Dictionary<Type, Type> requestHandlerMap)
        {
            _resolver = resolverContract ?? throw new ArgumentNullException(nameof(resolverContract));
            _requestHandlerMap = requestHandlerMap
                .ThrowIfNull(() => new ArgumentNullException(nameof(requestHandlerMap)))
                .WithEach(kvp => RequestManifestBuilder.ValidateHandlerMap(kvp, _requestNamespaceMap.ContainsKey))
                .WithEach(kvp => _requestNamespaceMap.Add(kvp.Key.InstructionNamespace(), kvp.Key))
                .ToDictionary();
        }

        /// <summary>
        /// Gets all the request namespaces.
        /// </summary>
        public InstructionNamespace[] Namespaces() => _requestNamespaceMap.Keys.ToArray();

        /// <summary>
        /// Gets all the request types
        /// </summary>
        public Type[] RequestTypes() => _requestHandlerMap.Keys.ToArray();

        /// <summary>
        /// Gets the request type mapped to the given namespace, or null if no mapping exists.
        /// </summary>
        /// <param name="namespace">The namespace</param>
        public Type? RequestTypeFor(InstructionNamespace @namespace)
            => _requestNamespaceMap.TryGetValue(@namespace, out var requestType)
                ? requestType
                : null;

        /// <summary>
        /// Gets the request handler mapped to the given <typeparamref name="TRequest"/>, or null if no mapping exists.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        public IRequestHandler<TRequest, TResult>? HandlerFor<TRequest, TResult>()
        where TRequest : IRequest<TResult>
        {
            return GetRequestHandlerTypeOrNull(typeof(TRequest)) switch
            {
                Type htype => _resolver
                    .Resolve(htype)
                    .As<IRequestHandler<TRequest, TResult>>(),
                _ => null
            };
        }

        private Type? GetRequestHandlerTypeOrNull(Type requestType)
            => _requestHandlerMap.TryGetValue(requestType, out var handlerType)
                ? handlerType
                : null;
    }
}
