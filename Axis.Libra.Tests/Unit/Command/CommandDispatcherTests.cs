using Axis.Libra.Command;
using Axis.Libra.Tests.TestCQRs.Commands;
using Axis.Libra.URI;
using Axis.Proteus.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Command
{
    [TestClass]
    public class CommandDispatcherTests
    {
        private Mock<IResolverContract> mockResolver = new Mock<IResolverContract>();

        [TestMethod]
        public async Task DispatchCommand_WithValidArgs_ShouldExecuteCommandHandler()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(new Command1Handler());
            var manifest = new CommandManifest(
                mockResolver.Object,
                new System.Collections.Generic.Dictionary<Type, Type>
                {
                    [typeof(Command1)] = typeof(Command1Handler)
                });
            var dispatcher = new CommandDispatcher(manifest);

            var result = await dispatcher.Dispatch(new Command1());
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task DispatchCommand_WithInvalidArgs_ShouldThrowException()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(null);
            var manifest = new CommandManifest(
                mockResolver.Object,
                new System.Collections.Generic.Dictionary<Type, Type>
                {
                    [typeof(Command1)] = typeof(Command1Handler)
                });
            var dispatcher = new CommandDispatcher(manifest);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dispatcher.Dispatch<Command1>(null));
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => dispatcher.Dispatch(new Command1()));
        }

        [TestMethod]
        public async Task DispatchStatusRequest_WithValidArgs_ShouldExecuteCommandHandler()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(new Command1Handler());
            var manifest = new CommandManifest(
                mockResolver.Object,
                new System.Collections.Generic.Dictionary<Type, Type>
                {
                    [typeof(Command1)] = typeof(Command1Handler)
                });
            var dispatcher = new CommandDispatcher(manifest);

            var result = await dispatcher.DispatchStatusRequest(
                new InstructionURI(
                    Scheme.Command,
                    typeof(Command1).InstructionNamespace(),
                    1ul));
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task DispatchStatusRequest_WithinvalidArgs_ShouldThrowException()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(null);
            var manifest = new CommandManifest(
                mockResolver.Object,
                new System.Collections.Generic.Dictionary<Type, Type>
                {
                    [typeof(Command1)] = typeof(Command1Handler)
                });
            var dispatcher = new CommandDispatcher(manifest);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => dispatcher.DispatchStatusRequest(default));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => dispatcher.DispatchStatusRequest(
                new InstructionURI(
                    Scheme.Request,
                    typeof(Command1).InstructionNamespace(),
                    1ul)));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => dispatcher.DispatchStatusRequest(
                new InstructionURI(
                    Scheme.Query,
                    typeof(Command1).InstructionNamespace(),
                    1ul)));
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => dispatcher.DispatchStatusRequest(
                new InstructionURI(
                    Scheme.Command,
                    typeof(Command1).InstructionNamespace(),
                    1ul)));
        }
    }
}
