using Axis.Libra.Instruction;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Builder for the <see cref="CommandManifest"/>
    /// </summary>
    public class CommandManifestBuilder
    {
        private readonly IRegistrarContract _contract;
        private readonly Dictionary<Type, Type> _commandHandlerMap;
        private readonly HashSet<InstructionNamespace> _instructionNamespaces;

        public CommandManifestBuilder(IRegistrarContract contract)
        {
            _contract = contract
                ?.ThrowIf(c => c.IsRegistrationClosed(), _ => new ArgumentException($"Invalid {nameof(contract)}: locked"))
                ?? throw new ArgumentNullException(nameof(contract));
            _commandHandlerMap = new Dictionary<Type, Type>();
            _instructionNamespaces = new HashSet<InstructionNamespace>();
        }

        /// <summary>
        /// Adds a command handler to the manifest, and also registers it with the underlying <see cref="IRegistrarContract"/>
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <typeparam name="TCommandHandler">The handler type</typeparam>
        /// <param name="scope">The optional IoC registration scope</param>
        /// <param name="interceptorProfile">The optional interceptors for the command handler</param>
        /// <returns></returns>
        public CommandManifestBuilder AddCommandHandler<TCommand, TCommandHandler>(
            ResolutionScope scope = default,
            InterceptorProfile interceptorProfile = default)
            where TCommand : ICommand
            where TCommandHandler: class, ICommandHandler<TCommand>
        {
            ValidateHandlerMap(
                typeof(TCommand).ValuePair(typeof(TCommandHandler)),
                @namespace => _instructionNamespaces.Contains(@namespace));

            // placing this here so failed registrations will throw exceptions and subsequent statements wont execute
            _ = _contract.Register<TCommandHandler>(scope, interceptorProfile);
            _commandHandlerMap.Add(typeof(TCommand), typeof(TCommandHandler));
            _instructionNamespaces.Add(typeof(TCommand).InstructionNamespace());

            return this;
        }

        internal (Type commandType, Type commandHandlerType)[] CommandMaps() => _commandHandlerMap
            .Select(kvp => (kvp.Key, kvp.Value))
            .ToArray();

        /// <summary>
        /// Creates a <see cref="CommandManifest"/> instance using the given resolver
        /// </summary>
        /// <param name="resolver">The service resolver</param>
        public CommandManifest BuildManifest(IResolverContract resolver)
        {
            return new CommandManifest(resolver, _commandHandlerMap);
        }

        /// <summary>
        /// check that the command map is valid:
        /// <list type="number">
        /// <item><see cref="ICommand"/> is non-null</item>
        /// <item>command-type implements <see cref="ICommand"/></item>
        /// <item>command-type is decorated with <see cref="InstructionNamespaceAttribute"/></item>
        /// <item><see cref="InstructionNamespaceAttribute"/> must be unique for each command</item>
        /// <item>handler-type implements <see cref="ICommandHandler{TCommand}"/>, where <c>TCommand</c> is command-type (key) in the <paramref name="commandMap"/>.</item>
        /// <item>handler type is non-null</item>
        /// <item>handler type implements <see cref="ICommandHandler{TCommand}"/></item>
        /// <item>handler must be a concrete class/struct</item>
        /// </list>
        /// </summary>
        /// <param name="commndMap">The map from command-type to command-handler-type</param>
        internal static void ValidateHandlerMap(
            KeyValuePair<Type, Type> commandMap,
            Func<InstructionNamespace, bool> containsNamespace)
        {
            var messagePrefix = "Invalid command-map:";

            if (containsNamespace == null)
                throw new ArgumentNullException(nameof(containsNamespace));

            #region command validation
            // command is non-null
            if (commandMap.Key is null)
                throw new InvalidOperationException($"{messagePrefix} command type cannot be null");

            // command-type implements ICommand
            if (!commandMap.Key.Implements(typeof(ICommand)))
                throw new InvalidOperationException($"{messagePrefix} command type does not implement {nameof(ICommand)}");

            // InstructionNamespaceAttribute must be unique for each query
            if (containsNamespace(commandMap.Key.InstructionNamespace()))
                throw new InvalidOperationException($"{messagePrefix} command type namespace '{commandMap.Key.InstructionNamespace()}' is not unique");
            #endregion

            #region command handler validation
            // handler-type is non-null
            if (commandMap.Value is null)
                throw new InvalidOperationException($"{messagePrefix} handler type cannot be null");

            // handler-type implements ICommandHandler<TCommand>
            var expectedInterface = typeof(ICommandHandler<>).MakeGenericType(commandMap.Key);
            if (!commandMap.Value.Implements(expectedInterface))
                throw new InvalidOperationException($"{messagePrefix} handler type does not implement {expectedInterface}");

            // handler must be a concrete class/struct. No need to check for delegates or enums, as those types cannot implement interfaces
            if (!(!commandMap.Value.IsAbstract
                && !commandMap.Value.IsGenericTypeDefinition
                && !commandMap.Value.IsGenericParameter))
                throw new InvalidOperationException($"{messagePrefix} handler type must be a concrete type");
            #endregion
        }
    }

    /// <summary>
    /// A manifest is built out of the complete discovery and registration of commands.
    /// It encapsulates a list of all <see cref="ICommandHandler{TCommand}"/>s and  <see cref="ICommand"/>s.
    /// </summary>
    public class CommandManifest
    {
        /// <summary>
        /// The service resolver
        /// </summary>
        private readonly IResolverContract _resolver;

        /// <summary>
        /// The namespace - command-type map
        /// </summary>
        private readonly Dictionary<InstructionNamespace, Type> _commandNamespaceMap = new();

        /// <summary>
        /// The command-type - command-handler-type map
        /// </summary>
        private readonly Dictionary<Type, Type> _commandHandlerMap;


        public CommandManifest(
            IResolverContract resolverContract,
            Dictionary<Type, Type> commandHandlerMap)
        {
            _resolver = resolverContract ?? throw new ArgumentNullException(nameof(resolverContract));
            _commandHandlerMap = commandHandlerMap
                .ThrowIfNull(() => new ArgumentNullException(nameof(commandHandlerMap)))
                .WithEach(kvp => CommandManifestBuilder.ValidateHandlerMap(kvp, _commandNamespaceMap.ContainsKey))
                .WithEach(kvp => _commandNamespaceMap.Add(kvp.Key.InstructionNamespace(), kvp.Key))
                .ToDictionary(ignoreDuplicates: false);
        }

        /// <summary>
        /// Gets all the command namespaces.
        /// </summary>
        public ImmutableArray<InstructionNamespace> Namespaces() => _commandNamespaceMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets all the command types
        /// </summary>
        public ImmutableArray<Type> CommandTypes() => _commandHandlerMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets the command type mapped to the given namespace, or null if no mapping exists.
        /// </summary>
        /// <param name="namespace">The namespace</param>
        public Type? CommandTypeFor(
            InstructionNamespace @namespace)
            => _commandNamespaceMap.TryGetValue(@namespace, out var commandType)
                ? commandType
                : null;

        /// <summary>
        /// Gets the command handler mapped to the given <typeparamref name="TCommand"/>, or null if no mapping exists.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        public ICommandHandler<TCommand>? HandlerFor<TCommand>()
        where TCommand : ICommand
        {
            return GetCommandHandlerTypeOrNull(typeof(TCommand)) switch
            {
                Type htype => _resolver
                    .Resolve(htype)
                    .As<ICommandHandler<TCommand>>(),
                _ => null
            };
        }

        /// <summary>
        /// Gets the status handler mapped to the given <paramref name="namespace"/>, or null if no mapping exists.
        /// </summary>
        /// <param name="namespace">the command namespace</param>
        public ICommandStatusHandler? StatusHandlerFor(InstructionNamespace @namespace)
        {
            return GetCommandTypeOrNull(@namespace) switch
            {
                Type htype => _resolver
                    .Resolve(htype)
                    .As<ICommandStatusHandler>(),
                _ => null
            };
        }


        private Type? GetCommandHandlerTypeOrNull(
            Type commandType)
            => _commandHandlerMap.TryGetValue(commandType, out var handlerType)
                    ? handlerType
                    : null;

        private Type? GetCommandTypeOrNull(
            InstructionNamespace @namespace)
            => _commandNamespaceMap.TryGetValue(@namespace, out var commandType)
                ? commandType
                : null;
    }
}
