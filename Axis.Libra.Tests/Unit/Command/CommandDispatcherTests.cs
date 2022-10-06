using Axis.Libra.Command;
using Axis.Libra.Exceptions;
using Axis.Libra.Tests.TestCQRs.Commands;
using Axis.Luna.Operation;
using Axis.Proteus.IoC;
using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Command
{
    using Registration = ServiceRegistrar.RegistrationMap;

    [TestClass]
    public class CommandDispatcherTests
    {
        [TestMethod]
        public async Task Dispatch_WithRegisteredCommand_ShouldCallHandler()
        {
            // setup
            var _mockHandler = new Mock<ICommandHandler<Command1>>();
            _mockHandler
                .Setup(h => h.ExecuteCommand(It.IsAny<Command1>()))
                .Returns((Command1 arg1) => Operation.FromResult(arg1.CommandURI))
                .Verifiable();

            var _mockResolver = new Mock<ServiceResolver>(
                new Mock<IResolverContract>().Object,
                new Mock<IProxyGenerator>().Object,
                Array.Empty<Registration>());
            _mockResolver
                .Setup(r => r.Resolve<ICommandHandler<Command1>>())
                .Returns(_mockHandler.Object)
                .Verifiable();

            var dispatcher = new CommandDispatcher(_mockResolver.Object);

            // test
            var command = new Command1 { Name = "Adolf", Description = "Baddest ever" };
            await dispatcher
                .Dispatch(command)
                .Then(signature =>
                {
                    // assert
                    _mockResolver.Verify();
                    _mockHandler.Verify();
                    Assert.AreEqual(command.CommandURI, signature);
                });
        }

        [TestMethod]
        public async Task Dispatch_WithNullCommand_ShouldThrowException()
        {
            // setup
            var _mockResolver = new Mock<ServiceResolver>(
                new Mock<IResolverContract>().Object,
                new Mock<IProxyGenerator>().Object,
                Array.Empty<Registration>());
            var dispatcher = new CommandDispatcher(_mockResolver.Object);

            // test
            var op = dispatcher.Dispatch<Command1>(null);

            // assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await op);
        }

        [TestMethod]
        public async Task Dispatch_WithUnregisteredCommand_ShouldThrowException()
        {
            // setup
            var _mockHandler = new Mock<ICommandHandler<Command1>>();
            var _mockResolver = new Mock<ServiceResolver>(
                new Mock<IResolverContract>().Object,
                new Mock<IProxyGenerator>().Object,
                Array.Empty<Registration>());

            _mockResolver
                .Setup(r => r.Resolve<ICommandHandler<Command1>>())
                .Throws(new SimpleInjector.ActivationException())
                .Verifiable();

            var dispatcher = new CommandDispatcher(_mockResolver.Object);

            // test
            var command = new Command1 { Name = "Adolf", Description = "Baddest ever" };
            var op = dispatcher.Dispatch(command);

            // assert
            await Assert.ThrowsExceptionAsync<SimpleInjector.ActivationException>(async () => await op);
            _mockResolver.Verify();
            _mockHandler.Verify();
        }

        [TestMethod]
        public async Task Dispatch_WhenResolverYieldsNull_ShouldThrowException()
        {
            // setup
            var _mockHandler = new Mock<ICommandHandler<Command1>>();
            var _mockResolver = new Mock<ServiceResolver>(
                new Mock<IResolverContract>().Object,
                new Mock<IProxyGenerator>().Object,
                Array.Empty<Registration>());

            _mockResolver
                .Setup(r => r.Resolve<ICommandHandler<Command1>>())
                .Returns<Command1>(null)
                .Verifiable();

            var dispatcher = new CommandDispatcher(_mockResolver.Object);

            // test
            var command = new Command1 { Name = "Adolf", Description = "Baddest ever" };
            var op = dispatcher.Dispatch(command);

            // assert
            await Assert.ThrowsExceptionAsync<UnknownResolverException>(async () => await op);
            _mockResolver.Verify();
            _mockHandler.Verify();
        }
    }
}
