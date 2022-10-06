using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Axis.Libra.Request
{
    /// <summary>
    /// Provides the service of registering and discovering commands, dispatchers and command result queries with a supplied <see cref="Proteus.IoC.ServiceRegistrar"/>.
    /// </summary>
    public class RequestRegistrar
    {
        /// <summary>
        /// The underlying IoC registrar.
        /// </summary>
        private IRegistrarContract IocRegistrar { get; }

        /// <summary>
        /// The registration cache. A dictionary of <see cref="IRequestHandler{TRequest}"/> types mapped to their implementations and registration information
        /// </summary>
        private Dictionary<Type, HashSet<(Type, RegistryScope?, InterceptorProfile?)>> RegistrationsCache { get; } = new Dictionary<Type, HashSet<(Type, RegistryScope?, InterceptorProfile?)>>();

        /// <summary>
        /// The manifest. Once built, the <see cref="RegistrationsCache"/> is purged permanently
        /// </summary>
        private RequestManifest Manifest { get; set; }

        /// <summary>
        /// Return a new <see cref="CommandRegistrar"/> that is ready to register commands.
        /// </summary>
        /// <param name="iocRegistrar">The underlying IoC Registrar into which the instances are registered</param>
        public static RequestRegistrar BeginRegistration(IRegistrarContract iocRegistrar) => new RequestRegistrar(iocRegistrar);

        private RequestRegistrar(IRegistrarContract iocRegistrar)
        {
            IocRegistrar = iocRegistrar ?? throw new ArgumentNullException(nameof(iocRegistrar));
        }


        /// <summary>
        /// Register a specific request handler type.
        /// </summary>
        /// <typeparam name="THandler">The concrete handler implementation type</typeparam>
        /// <typeparam name="TCommand">The concrete request type</typeparam>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public RequestRegistrar AddHandlerRegistration<THandlerImplementation, TRequest>(
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        where TRequest : IRequest
        where THandlerImplementation : class, IRequestHandler<IRequest>
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            _ = typeof(TRequest).ValidateInstructionType();

            RegistrationsCache
                .GetOrAdd(typeof(IRequestHandler<TRequest>), key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                .Add((typeof(THandlerImplementation), scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Register a specific request handler type
        /// </summary>
        /// <param name="requestHandlerImplementationType">the request handler implementation</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public RequestRegistrar AddHandlerRegistration(
            Type requestHandlerImplementationType,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            requestHandlerImplementationType
                .ValidateRequestHandlerImplementation()
                .GetInterfaces()
                .Where(@interface => @interface.IsGenericType)
                .Where(@interface => @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                // for each of the implemented request handlers, register this implementation type
                .ForAll(@interface =>
                {
                    RegistrationsCache
                        .GetOrAdd(@interface, key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                        .Add((requestHandlerImplementationType, scope, interceptorProfile));
                });

            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="IRequestHandler{TRequest}"/> found within the given namespace alone, for the assembly of the CALLING method
        /// </summary>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="recursiveSearch">Indicates if recursive namespace search is required</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public RequestRegistrar AddNamespaceHandlerRegistrations(
            string @namespace,
            bool recursiveSearch = false,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            return AddNamespaceHandlerRegistrations(
                new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly,
                @namespace,
                recursiveSearch,
                scope,
                interceptorProfile);
        }

        /// <summary>
        /// Registers all instances of <see cref="IRequestHandler{TRequest}"/> found within the given namespace alone, for the supplied assembly.
        /// </summary>
        /// <param name="assembly">assembly to search within</param>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="recursiveSearch">Indicates if recursive namespace search is required</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public RequestRegistrar AddNamespaceHandlerRegistrations(
            Assembly assembly,
            string @namespace,
            bool recursiveSearch = false,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            @namespace.ThrowIf(
                string.IsNullOrWhiteSpace,
                new ArgumentException($"Invalid {nameof(@namespace)}"));

            assembly
                .ThrowIfNull(new ArgumentNullException(nameof(assembly)))
                .GetExportedTypes()
                .Where(t => recursiveSearch ? t.Namespace.IsChildNamespaceOf(@namespace) : t.Namespace.Equals(@namespace, StringComparison.InvariantCulture))
                .Where(t => t.ImplementsGenericInterface(typeof(IRequestHandler<>)))
                .ForAll(t => AddHandlerRegistration(t, scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="IRequestHandler{TRequest}"/> found within the supplied assembly.
        /// </summary>
        /// <param name="assembly">assembly to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public RequestRegistrar AddAssemblyHandlerRegistrations(
            Assembly assembly,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            assembly.ThrowIfNull(new ArgumentNullException(nameof(assembly)));

            assembly
                .GetExportedTypes()
                .Where(t => t.ImplementsGenericInterface(typeof(IRequestHandler<>)))
                .ForAll(t => AddHandlerRegistration(t, scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Commits all the added registrations to the underlying IoC container, and builds a manifest for the commited registrations.
        /// </summary>
        /// <returns>the command manifest</returns>
        public RequestManifest CommitRegistrations()
        {
            if (Manifest == null)
            {
                FinalizeRegistrations();
                Manifest = new RequestManifest(RegistrationsCache.Keys);
                RegistrationsCache.Clear();
            }

            return Manifest;
        }

        private void FinalizeRegistrations()
        {
            foreach (var kvp in RegistrationsCache)
            {
                foreach (var registration in kvp.Value)
                    _ = IocRegistrar.Register(
                        kvp.Key,
                        registration.Item1,
                        registration.Item2 ?? default,
                        registration.Item3 ?? default);
            }
        }

    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of commands.
    /// It encapsulates a list of all <see cref="IRequestHandler{TRequest}"/>s, and <see cref="IRequest"/>s.
    /// </summary>
    public class RequestManifest
    {
        /// <summary>
        /// All request handlers registered in the registry
        /// </summary>
        public IEnumerable<Type> RequestHandlers { get; }

        /// <summary>
        /// All requests registered to handlers in the registry
        /// </summary>
        public IEnumerable<Type> Requests { get; }

        internal RequestManifest(IEnumerable<Type> requestHandlerTypes)
        {
            (RequestHandlers, Requests) = requestHandlerTypes.Aggregate((handlers: new List<Type>(), requests: new List<Type>()), (lists, next) =>
            {
                lists.handlers.Add(next);
                lists.requests.Add(next.GetGenericArguments()[0]);

                return lists;
            });
        }
    }
}
