using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Provides the service of discovering and registering commands and dispatchers and command result queries with a supplied <see cref="Proteus.IoC.ServiceRegistrar"/>.
    /// 
    /// Note that the command registrar ensures that the correct <see cref="Utils.BindCommandResultAttribute"/> bindings are respected during registration
    /// </summary>
    public class CommandRegistrar
    {
        /// <summary>
        /// The underlying IoC registrar.
        /// </summary>
        private ServiceRegistrar IocRegistrar { get; }

        /// <summary>
        /// The registration cache. A dictionary of <see cref="ICommandHandler{TCommand}"/> types mapped to their implementations and registration information
        /// </summary>
        private Dictionary<Type, HashSet<(Type, RegistryScope?, InterceptorProfile?)>> RegistrationsCache { get; } = new Dictionary<Type, HashSet<(Type, RegistryScope?, InterceptorProfile?)>>();

        /// <summary>
        /// The manifest. Once built, the <see cref="RegistrationsCache"/> is purged permanently
        /// </summary>
        private CommandManifest Manifest { get; set; }

        /// <summary>
        /// Return a new <see cref="CommandRegistrar"/> that is ready to register commands.
        /// </summary>
        /// <param name="iocRegistrar">The underlying IoC Registrar into which the instances are registered</param>
        public static CommandRegistrar BeginRegistration(ServiceRegistrar iocRegistrar) => new CommandRegistrar(iocRegistrar);


        private CommandRegistrar(ServiceRegistrar iocRegistrar)
        {
            IocRegistrar = iocRegistrar ?? throw new ArgumentNullException(nameof(iocRegistrar));
        }

        /// <summary>
        /// Register a specific command handler type.
        /// </summary>
        /// <typeparam name="THandler">The concrete handler implementation type</typeparam>
        /// <typeparam name="TCommand">The concrete command type</typeparam>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public CommandRegistrar AddHandlerRegistration<THandler, TCommand>(
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        where TCommand: ICommand
        where THandler: class, ICommandHandler<TCommand>
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            typeof(ICommand).ValidateCommandType();

            RegistrationsCache
                .GetOrAdd(typeof(ICommandHandler<TCommand>), key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                .Add((typeof(THandler), scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Register a specific command handler type
        /// </summary>
        /// <param name="commandHandlerType"></param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public CommandRegistrar AddHandlerRegistration(
            Type commandHandlerType,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            commandHandlerType
                .ThrowIfNull(new ArgumentNullException(nameof(commandHandlerType)))
                .ThrowIf(
                    t => !t.ImplementsGenericInterface(typeof(ICommandHandler<>)),
                    new ArgumentException($"Type must implement {typeof(ICommandHandler<>)}"))
                .GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .ForAll(@interface =>
                {
                    RegistrationsCache
                        .GetOrAdd(@interface, key => new HashSet<(Type, RegistryScope?, InterceptorProfile?)>())
                        .Add((commandHandlerType, scope, interceptorProfile));
                });

            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="ICommandHandler{TCommand}"/> found within the given namespace alone, for the assembly of the CALLING method
        /// </summary>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public CommandRegistrar AddNamespaceHandlerRegistrations(
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
        /// Registers all instances of <see cref="ICommandHandler{TCommand}"/> found within the given namespace alone, for the supplied assembly.
        /// </summary>
        /// <param name="assembly">assembly to search within</param>
        /// <param name="namespace">the namespace to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public CommandRegistrar AddNamespaceHandlerRegistrations(
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
                .Where(t => t.ImplementsGenericInterface(typeof(ICommandHandler<>)))
                .ForAll(t => AddHandlerRegistration(t, scope, interceptorProfile));

            return this;
        }

        /// <summary>
        /// Registers all instances of <see cref="ICommandHandler{TCommand}"/> found within the supplied assembly.
        /// </summary>
        /// <param name="assembly">assembly to search within</param>
        /// <param name="scope">registration scope</param>
        /// <param name="interceptorProfile">interception profile applied to resolved instances of this registration</param>
        /// <returns>this instance of the registrar</returns>
        public CommandRegistrar AddAssemblyHandlerRegistrations(
            Assembly assembly,
            RegistryScope? scope = null,
            InterceptorProfile? interceptorProfile = null)
        {
            if (Manifest != null)
                throw new InvalidOperationException("Cannot add new registrations after manifest has been built");

            assembly.ThrowIfNull(new ArgumentNullException(nameof(assembly)));

            assembly
                .GetExportedTypes()
                .Where(t => t.ImplementsGenericInterface(typeof(ICommandHandler<>)))
                .ForAll(t => AddHandlerRegistration(t, scope, interceptorProfile));

            return this;
        }


        public CommandManifest BuildManifest()
        {
            if(Manifest == null)
            {
                FinalizeRegistrations();
                Manifest = new CommandManifest(RegistrationsCache.Keys);
                RegistrationsCache.Clear();
            }

            return Manifest;
        }

        private void FinalizeRegistrations()
        {
            foreach(var kvp in RegistrationsCache)
            {
                foreach(var registration in kvp.Value)
                    IocRegistrar.Register(kvp.Key, registration.Item1, registration.Item2);

                //IocRegistrar.RegisterCollection(kvp.Key, registration.Item1, registration.Item2, kvp.Value);
            }
        }
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of commands.
    /// It encapsulates a list of all <see cref="ICommandHandler{TCommand}"/>s,  <see cref="ICommand"/>s, and corresponding <see cref="ICommandResult"/>s in the system.
    /// </summary>
    public class CommandManifest
    {
        public IEnumerable<CommandInfo> Commands { get; }

        public IEnumerable<Type> CommandHandlers { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandManifest"/>
        /// </summary>
        /// <param name="types">List of <see cref="ICommandHandler{TCommand}"/> interface types.</param>
        internal CommandManifest(IEnumerable<Type> types)
        {
            (CommandHandlers, Commands) = types.Aggregate((new List<Type>(), new List<CommandInfo>()), ((lists, next) =>
            {
                lists.Item1.Add(next);

                var commandType = next
                    .GetGenericInterface(typeof(ICommandHandler<>))
                    .GetGenericArguments()[0];
                var commandResutType = commandType
                    .GetAttribute<BindCommandResultAttribute>()
                    .ResultType;

                lists.Item2.Add(new CommandInfo(commandType, commandResutType));

                return lists;
            }));
        }


        public struct CommandInfo
        {
            public Type CommandType { get; }

            public Type CommandResultType { get; }

            public CommandInfo(Type commandType, Type commandResultType)
            {
                CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
                CommandResultType = commandResultType ?? throw new ArgumentNullException(nameof(commandResultType));
            }

            public override int GetHashCode() => HashCode.Combine(CommandType, CommandResultType);

            public override bool Equals(object obj)
            {
                return obj is CommandInfo other
                    && other.CommandType == CommandType
                    && other.CommandResultType == CommandResultType;
            }
        }
    }
}
