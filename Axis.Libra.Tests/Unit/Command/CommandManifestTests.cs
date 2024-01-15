using Axis.Libra.Command;
using Axis.Libra.Tests.TestCQRs.Commands;
using Axis.Libra.Instruction;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Command
{
    [TestClass]
    public class CommandManifestBuilderTests
    {
        private Moq.Mock<IRegistrarContract> mockRegistrar = new Moq.Mock<IRegistrarContract>();

        [TestMethod]
        public void Constructor_ShouldCreateValidInstance()
        {
            var instance = new CommandManifestBuilder(mockRegistrar.Object);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowException()
        {
            mockRegistrar
                .Setup(r => r.IsRegistrationClosed())
                .Returns(true);

            Assert.ThrowsException<ArgumentNullException>(() => new CommandManifestBuilder(null));
            Assert.ThrowsException<ArgumentException>(() => new CommandManifestBuilder(mockRegistrar.Object));
        }

        [TestMethod]
        public void AddCommandHandler_ShouldRegister_AndAddMap()
        {
            // arrange
            mockRegistrar
                .Setup(r => r.Register<TestCQRs.Commands.Command1Handler>(
                    It.IsAny<RegistryScope>(),
                    It.IsAny<InterceptorProfile>(),
                    It.IsAny<IBindContext[]>()))
                .Returns(mockRegistrar.Object)
                .Verifiable();

            var builder = new CommandManifestBuilder(mockRegistrar.Object);

            // act
            var returned = builder.AddCommandHandler<TestCQRs.Commands.Command1, TestCQRs.Commands.Command1Handler>();

            // assert
            Assert.IsTrue(builder
                .CommandMaps()
                .Any(m => m.commandType == typeof(TestCQRs.Commands.Command1)
                    && m.commandHandlerType == typeof(TestCQRs.Commands.Command1Handler)));
            Assert.AreEqual(returned, builder);
            mockRegistrar.Verify();
        }

        [TestMethod]
        public void ValidateHandlerMap_ShouldValidate()
        {
            // valid args
            CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair(typeof(TestCQRs.Commands.Command1Handler)),
                ns => false);

            #region command
            // null predicate
            Assert.ThrowsException<ArgumentNullException>(() => CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair(typeof(TestCQRs.Commands.Command1Handler)),
                null));

            // null-command type
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                ((Type)null).ValuePair(typeof(TestCQRs.Commands.Command1Handler)),
                ns => false));

            // doesn't implement the ICommand interface
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                (typeof(object)).ValuePair(typeof(TestCQRs.Commands.Command1Handler)),
                ns => false));

            // non-decorated command type
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                (typeof(NonDecoratedCommand)).ValuePair(typeof(TestCQRs.Commands.Command1Handler)),
                ns => false));

            // non-unique namespace command type
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                (typeof(TestCQRs.Commands.Command1)).ValuePair(typeof(TestCQRs.Commands.Command1Handler)),
                ns => true));
            #endregion

            #region command handler
            // null handler-type
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair((Type)null),
                ns => false));

            // doesn't implement the ICommandHandler<> interface
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair(typeof(object)),
                ns => false));

            // doesn't implement the ICommandHandler<> interface
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair(typeof(TestCQRs.Commands.Command2Handler)),
                ns => false));

            // is not a concrete type
            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair(typeof(ICommand1Handler2)),
                ns => false));

            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair(typeof(Command1Handler2)),
                ns => false));

            Assert.ThrowsException<InvalidOperationException>(() => CommandManifestBuilder.ValidateHandlerMap(
                typeof(TestCQRs.Commands.Command1).ValuePair(typeof(GenericCommand1Handler3<>)),
                ns => false));
            #endregion
        }
    }

    [TestClass]
    public class CommandManifestTest
    {
        private Mock<IResolverContract> mockResolver = new Moq.Mock<IResolverContract>();

        [TestMethod]
        public void Constructor_ShouldReturnValidInstance()
        {
            var manifest = new CommandManifest(
                mockResolver.Object,
                new Dictionary<Type, Type>());
            Assert.IsNotNull(manifest);

            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Command1)] = typeof(Command1Handler)
            };
            manifest = new CommandManifest(mockResolver.Object, handlerMap);
            Assert.IsNotNull(manifest);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowException()
        {
            var handlerMap = new Dictionary<Type, Type>();

            Assert.ThrowsException<ArgumentNullException>(() => new CommandManifest(null, handlerMap));
            Assert.ThrowsException<ArgumentNullException>(() => new CommandManifest(mockResolver.Object, null));

            handlerMap[typeof(Command1)] = typeof(Command1Handler);
            handlerMap[typeof(Command1)] = typeof(Command2Handler);

            Assert.ThrowsException<InvalidOperationException>(() => new CommandManifest(mockResolver.Object, handlerMap));
        }

        [TestMethod]
        public void Namespaces_ShouldReturnAllCommandNamespaces()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Command1)] = typeof(Command1Handler),
                [typeof(Command2)] = typeof(Command2Handler)
            };
            var manifest = new CommandManifest(mockResolver.Object, handlerMap);

            Assert.IsTrue(
                manifest
                    .Namespaces()
                    .SequenceEqual(new[] { typeof(Command1).InstructionNamespace(), typeof(Command2).InstructionNamespace() }));
        }

        [TestMethod]
        public void CommandTypes_ShouldReturnAllCommanTypes()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Command1)] = typeof(Command1Handler),
                [typeof(Command2)] = typeof(Command2Handler)
            };
            var manifest = new CommandManifest(mockResolver.Object, handlerMap);

            Assert.IsTrue(
                manifest
                    .CommandTypes()
                    .SequenceEqual(new[] { typeof(Command1), typeof(Command2) }));
        }

        [TestMethod]
        public void CommandTypeFor_ShouldReturnCorrectCommandType()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Command1)] = typeof(Command1Handler),
                [typeof(Command2)] = typeof(Command2Handler)
            };
            var manifest = new CommandManifest(mockResolver.Object, handlerMap);

            var commandType = manifest.CommandTypeFor(typeof(Command1).InstructionNamespace());
            Assert.AreEqual(typeof(Command1), commandType);
        }

        [TestMethod]
        public void HandlerFor_ShouldReturnCorrectHandlerInstance()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(new Command1Handler());
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Command1)] = typeof(Command1Handler),
                [typeof(Command2)] = typeof(Command2Handler)
            };
            var manifest = new CommandManifest(mockResolver.Object, handlerMap);

            var handlerInstance = manifest.HandlerFor<Command1>();
            Assert.IsNotNull(handlerInstance);
        }

        [TestMethod]
        public void StatusHandlerFor_ShouldReturnCorrectHandlerInstance()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(new Command1Handler());
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Command1)] = typeof(Command1Handler),
                [typeof(Command2)] = typeof(Command2Handler)
            };
            var manifest = new CommandManifest(mockResolver.Object, handlerMap);

            var handlerInstance = manifest.StatusHandlerFor(typeof(Command2).InstructionNamespace());
            Assert.IsNotNull(handlerInstance);
        }
    }

    public class NonDecoratedCommand : ICommand
    {
        public InstructionURI InstructionURI => default;
    }

    public interface ICommand1Handler2: ICommandHandler<TestCQRs.Commands.Command1>
    { }

    public abstract class Command1Handler2: TestCQRs.Commands.Command1Handler
    { }

    public class GenericCommand1Handler3<T> : ICommandHandler<TestCQRs.Commands.Command1>
    {
        public Task<IResult<InstructionURI>> ExecuteCommand(Command1 command)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<ICommandStatus>> ExecuteSatusRequest(InstructionURI commandURI)
        {
            throw new NotImplementedException();
        }
    }
}
