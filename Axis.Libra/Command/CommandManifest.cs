using Axis.Libra.Instruction;
using Axis.Libra.Utils;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Axis.Libra.Command
{
    /// <summary>
    /// Builder for the <see cref="CommandManifest"/>
    /// </summary>
    public class CommandManifestBuilder
    {
        private readonly List<CommandInfo> _commandInfoList = new();
        private readonly HashSet<InstructionNamespace> _namespaces = new();
        private readonly HashSet<Type> _commandTypes = new();

        internal ImmutableArray<CommandInfo> CommandInfoList => _commandInfoList.ToImmutableArray();

        public CommandManifestBuilder()
        { }

        public static CommandManifestBuilder NewBuilder() => new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TCommandHandler"></typeparam>
        /// <typeparam name="TStatusHandler"></typeparam>
        /// <param name="commandHandler"></param>
        /// <param name="statusHandler"></param>
        /// <param name="commandHasher"></param>
        /// <param name="namespace"></param>
        public CommandManifestBuilder AddCommandHandler<TCommand, TCommandHandler, TStatusHandler>(
            Func<TCommand, InstructionURI, TCommandHandler, Task> commandHandler,
            Func<InstructionURI, TStatusHandler, Task<ICommandStatus>> statusHandler,
            Func<TCommand, ulong>? commandHasher = null,
            InstructionNamespace @namespace = default)
        {
            ArgumentNullException.ThrowIfNull(commandHandler);
            ArgumentNullException.ThrowIfNull(statusHandler);

            var _namespace = @namespace.IsDefault
                ? typeof(TCommand).DefaultInstructionNamespace("Command")
                : @namespace;

            Func<object, ulong> _hasher = commandHasher is not null
                ? cmdObj => commandHasher.Invoke((TCommand)cmdObj)
                : PropertySerializer.HashProperties;

            if (!_namespaces.Add(_namespace))
                throw new InvalidOperationException(
                    $"Invalid {nameof(@namespace)}: duplicate value '{@namespace}'");

            if (!_commandTypes.Add(typeof(TCommand)))
                throw new InvalidOperationException(
                    $"Invalid command type: duplicate value '{typeof(TCommand)}'");

            var info = new CommandInfo(
                _namespace,
                typeof(TCommand),
                typeof(TCommandHandler),
                typeof(TStatusHandler),
                (cmdObj, uri, hnldObj) => commandHandler.Invoke((TCommand)cmdObj, uri, (TCommandHandler)hnldObj),
                (uri, hnldObj) => statusHandler.Invoke(uri, (TStatusHandler)hnldObj),
                _hasher);
            _commandInfoList.Add(info);

            return this;
        }

        /// <summary>
        /// Creates a <see cref="CommandManifest"/> instanc
        /// </summary>
        public CommandManifest BuildManifest() => new(_commandInfoList);
    }

    /// <summary>
    /// A manifest is built out of the completed registration of commands.
    /// </summary>
    public class CommandManifest
    {
        /// <summary>
        /// The namespace - command-type map
        /// </summary>
        private readonly Dictionary<InstructionNamespace, CommandInfo> _commandNamespaceMap = new();

        /// <summary>
        /// The command-type - command-handler-type map
        /// </summary>
        private readonly Dictionary<Type, CommandInfo> _commandTypeMap = new();

        internal CommandManifest(IEnumerable<CommandInfo> infoList)
        {
            infoList
                .ThrowIfNull(() => new ArgumentNullException(nameof(infoList)))
                .ThrowIfAny(
                    info => info.IsDefault,
                    _ => new ArgumentException($"Invalid {nameof(infoList)}: contains default"))
                .ForAll(info =>
                {
                    _commandNamespaceMap.Add(info.Namespace, info);
                    _commandTypeMap.Add(info.CommandType, info);
                });
        }

        /// <summary>
        /// Gets all the command namespaces.
        /// </summary>
        public ImmutableArray<InstructionNamespace> Namespaces() => _commandNamespaceMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets all the command types
        /// </summary>
        public ImmutableArray<Type> CommandTypes() => _commandTypeMap.Keys.ToImmutableArray();

        /// <summary>
        /// Gets the command type mapped to the given command type, or null if no mapping exists.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        internal CommandInfo? GetCommandInfo<TCommand>()
            => _commandTypeMap.TryGetValue(typeof(TCommand), out var commandInfo)
                ? commandInfo
                : null;

        /// <summary>
        /// Gets the command type mapped to the given namespace, or null if no mapping exists.
        /// </summary>
        /// <param name="namespace">The namespace</param>
        internal CommandInfo? GetCommandInfo(InstructionNamespace @namespace)
            => _commandNamespaceMap.TryGetValue(@namespace, out var commandInfo)
                ? commandInfo
                : null;
    }
}
