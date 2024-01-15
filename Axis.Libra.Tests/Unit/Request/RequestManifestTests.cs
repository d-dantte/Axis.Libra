using Axis.Libra.Request;
using Axis.Libra.Tests.TestCQRs.Requests;
using Axis.Libra.Instruction;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Request
{
    [TestClass]
    public class RequestManifestBuilderTests
    {
        private Moq.Mock<IRegistrarContract> mockRegistrar = new Moq.Mock<IRegistrarContract>();

        [TestMethod]
        public void Constructor_ShouldCreateValidInstance()
        {
            var instance = new RequestManifestBuilder(mockRegistrar.Object);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowException()
        {
            mockRegistrar
                .Setup(r => r.IsRegistrationClosed())
                .Returns(true);

            Assert.ThrowsException<ArgumentNullException>(() => new RequestManifestBuilder(null));
            Assert.ThrowsException<ArgumentException>(() => new RequestManifestBuilder(mockRegistrar.Object));
        }

        [TestMethod]
        public void AddRequestHandler_ShouldRegister_AndAddMap()
        {
            // arrange
            mockRegistrar
                .Setup(r => r.Register<Request1Handler>(
                    It.IsAny<RegistryScope>(),
                    It.IsAny<InterceptorProfile>(),
                    It.IsAny<IBindContext[]>()))
                .Returns(mockRegistrar.Object)
                .Verifiable();

            var builder = new RequestManifestBuilder(mockRegistrar.Object);

            // act
            var returned = builder.AddRequestHandler<Request1, Request1Result, Request1Handler>();

            // assert
            Assert.IsTrue(builder
                .RequestMaps()
                .Any(m => m.queryType == typeof(Request1)
                    && m.queryHandlerType == typeof(Request1Handler)));
            Assert.AreEqual(returned, builder);
            mockRegistrar.Verify();
        }

        [TestMethod]
        public void ValidateHandlerMap_ShouldValidate()
        {
            // valid args
            RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair(typeof(Request1Handler)),
                ns => false);

            #region query
            // null predicate
            Assert.ThrowsException<ArgumentNullException>(() => RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair(typeof(Request1Handler)),
                null));

            // null-query type
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                ((Type)null).ValuePair(typeof(Request1Handler)),
                ns => false));

            // non-decorated query type
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                (typeof(NonDecoratedRequest)).ValuePair(typeof(Request1Handler)),
                ns => false));

            // doesn't implement the IRequest interface
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                (typeof(object)).ValuePair(typeof(Request1Handler)),
                ns => false));

            // non-unique namespace query type
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                (typeof(Request1)).ValuePair(typeof(Request1Handler)),
                ns => true));
            #endregion

            #region query handler
            // null handler-type
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair((Type)null),
                ns => false));

            // doesn't implement the IRequestHandler<> interface
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair(typeof(object)),
                ns => false));

            // doesn't implement the IRequestHandler<> interface
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair(typeof(Request2Handler)),
                ns => false));

            // is not a concrete type
            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair(typeof(IRequest1Handler2)),
                ns => false));

            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair(typeof(Request1Handler2)),
                ns => false));

            Assert.ThrowsException<InvalidOperationException>(() => RequestManifestBuilder.ValidateHandlerMap(
                typeof(Request1).ValuePair(typeof(GenericRequest1Handler3<>)),
                ns => false));
            #endregion
        }
    }

    [TestClass]
    public class RequestManifestTest
    {
        private Mock<IResolverContract> mockResolver = new Moq.Mock<IResolverContract>();

        [TestMethod]
        public void Constructor_ShouldReturnValidInstance()
        {
            var manifest = new RequestManifest(
                mockResolver.Object,
                new Dictionary<Type, Type>());
            Assert.IsNotNull(manifest);

            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Request1)] = typeof(Request1Handler)
            };
            manifest = new RequestManifest(mockResolver.Object, handlerMap);
            Assert.IsNotNull(manifest);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowException()
        {
            var handlerMap = new Dictionary<Type, Type>();

            Assert.ThrowsException<ArgumentNullException>(() => new RequestManifest(null, handlerMap));
            Assert.ThrowsException<ArgumentNullException>(() => new RequestManifest(mockResolver.Object, null));

            handlerMap[typeof(Request1)] = typeof(Request1Handler);
            handlerMap[typeof(Request1)] = typeof(Request2Handler);

            Assert.ThrowsException<InvalidOperationException>(() => new RequestManifest(mockResolver.Object, handlerMap));
        }

        [TestMethod]
        public void Namespaces_ShouldReturnAllRequestNamespaces()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Request1)] = typeof(Request1Handler),
                [typeof(Request2)] = typeof(Request2Handler)
            };
            var manifest = new RequestManifest(mockResolver.Object, handlerMap);

            Assert.IsTrue(
                manifest
                    .Namespaces()
                    .SequenceEqual(new[] { typeof(Request1).InstructionNamespace(), typeof(Request2).InstructionNamespace() }));
        }

        [TestMethod]
        public void RequestTypes_ShouldReturnAllCommanTypes()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Request1)] = typeof(Request1Handler),
                [typeof(Request2)] = typeof(Request2Handler)
            };
            var manifest = new RequestManifest(mockResolver.Object, handlerMap);

            Assert.IsTrue(
                manifest
                    .RequestTypes()
                    .SequenceEqual(new[] { typeof(Request1), typeof(Request2) }));
        }

        [TestMethod]
        public void RequestTypeFor_ShouldReturnCorrectRequestType()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Request1)] = typeof(Request1Handler),
                [typeof(Request2)] = typeof(Request2Handler)
            };
            var manifest = new RequestManifest(mockResolver.Object, handlerMap);

            var queryType = manifest.RequestTypeFor(typeof(Request1).InstructionNamespace());
            Assert.AreEqual(typeof(Request1), queryType);
        }

        [TestMethod]
        public void HandlerFor_ShouldReturnCorrectHandlerInstance()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(new Request1Handler());
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Request1)] = typeof(Request1Handler),
                [typeof(Request2)] = typeof(Request2Handler)
            };
            var manifest = new RequestManifest(mockResolver.Object, handlerMap);

            var handlerInstance = manifest.HandlerFor<Request1, Request1Result>();
            Assert.IsNotNull(handlerInstance);
        }
    }

    public class NonDecoratedRequest : IRequest<int>
    {
        public InstructionURI InstructionURI => default;
    }

    public interface IRequest1Handler2 : IRequestHandler<Request1, Request1Result>
    { }

    public abstract class Request1Handler2 : Request1Handler
    { }

    public class GenericRequest1Handler3<T> : IRequestHandler<Request1, Request1Result>
    {
        public Task<Luna.Common.IResult<Request1Result>> ExecuteRequest(Request1 query)
        {
            throw new NotImplementedException();
        }
    }
}
