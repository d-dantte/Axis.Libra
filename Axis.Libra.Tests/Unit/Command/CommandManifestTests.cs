using Axis.Libra.Command;
using Axis.Libra.Tests.TestCQRs.Commands;
using Axis.Libra.Instruction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Axis.Libra.Tests.TestCQRs.Commands.Inner;

namespace Axis.Libra.Tests.Unit.Command
{
    [TestClass]
    public class CommandManifestBuilderTests
    {

        [TestMethod]
        public void AddCommandHandler_ShouldRegister_AndAddMap()
        {
            // arrange
            var builder = new CommandManifestBuilder();

            // act
            var returnedBuilder1 = builder.AddCommandHandler<Command1, Command1Handler, Command1Handler>(
                (cmd, uri, handler) => handler.ExecuteCommand(cmd),
                (uri, handler) => handler.ExecuteSatusRequest(uri));

            var returnedBuilder2 = builder.AddCommandHandler<Command2, Command2Handler, Command2Handler>(
                (cmd, uri, handler) => handler.ExecuteCommand(cmd),
                (uri, handler) => handler.ExecuteSatusRequest(uri),
                (cmd) => cmd.InstructionHash(),
                Command2.InstructionNamespace());

            // test

            // 1. reject duplicates
            Assert.ThrowsException<InvalidOperationException>(() =>
                builder.AddCommandHandler<Command1, Command1Handler, Command1Handler>(
                    (cmd, uri, handler) => handler.ExecuteCommand(cmd),
                    (uri, handler) => handler.ExecuteSatusRequest(uri)));

            // 2. returned builders are same as original builder
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder1));
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder2));

            // 3. builder has 2 elements
            Assert.AreEqual(2, builder.CommandInfoList.Length);
        }
    }

    [TestClass]
    public class CommandManifestTest
    {
        [TestMethod]
        public void Constructor_ShouldReturnValidInstance()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CommandManifest(null));
            Assert.ThrowsException<ArgumentException>(() => new CommandManifest(
                new[] { default(CommandInfo)}));
        }

        [TestMethod]
        public void Namespaces_ShouldReturnAllCommandNamespaces()
        {
            var manifest = new CommandManifest(GetCommandInfoList());
            var namespaces = manifest.Namespaces();

            Assert.AreEqual(3, namespaces.Length);
        }

        [TestMethod]
        public void CommandTypes_ShouldReturnAllCommanTypes()
        {
            var manifest = new CommandManifest(GetCommandInfoList());
            var types = manifest.CommandTypes();

            Assert.AreEqual(3, types.Length);
        }

        [TestMethod]
        public void GetCommandInfo_ShouldReturnCorrectInfo()
        {
            var manifest = new CommandManifest(GetCommandInfoList());

            var info = manifest.GetCommandInfo<Command1>();
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(Command1), info.Value.CommandType);

            info = manifest.GetCommandInfo(Command1.InstructionNamespace());
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(Command1), info.Value.CommandType);
            Assert.AreEqual(Command1.InstructionNamespace(), info.Value.Namespace);
            Assert.AreEqual(typeof(Command1Handler), info.Value.HandlerType);
            Assert.AreEqual(typeof(Command1Handler), info.Value.StatusRequestHandlerType);
        }

        private ImmutableArray<CommandInfo> GetCommandInfoList()
        {
            return ImmutableArray.Create(
                new CommandInfo(
                    Command1.InstructionNamespace(),
                    typeof(Command1),
                    typeof(Command1Handler),
                    typeof(Command1Handler),
                    (x, y, z) => Task.CompletedTask,
                    (x, y) => Task.FromResult(ICommandStatus.OfSuccess(x)),
                    (x) => 0),
                new CommandInfo(
                    Command2.InstructionNamespace(),
                    typeof(Command2),
                    typeof(Command2Handler),
                    typeof(Command2Handler),
                    (x, y, z) => Task.CompletedTask,
                    (x, y) => Task.FromResult(ICommandStatus.OfSuccess(x)),
                    (x) => 0),
                new CommandInfo(
                    new InstructionNamespace("Command3.Namespace"),
                    typeof(Command3),
                    typeof(Command3Handler),
                    typeof(Command3Handler),
                    (x, y, z) => Task.CompletedTask,
                    (x, y) => Task.FromResult(ICommandStatus.OfSuccess(x)),
                    (x) => 0));
        }
    }
}
