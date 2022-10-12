using Axis.Libra.Query;
using Axis.Libra.Tests.TestCQRs.Queries;
using Axis.Libra.URI;
using Axis.Luna.Extensions;
using Axis.Proteus.Interception;
using Axis.Proteus.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Query
{
    [TestClass]
    public class QueryManifestBuilderTests
    {
        private Moq.Mock<IRegistrarContract> mockRegistrar = new Moq.Mock<IRegistrarContract>();

        [TestMethod]
        public void Constructor_ShouldCreateValidInstance()
        {
            var instance = new QueryManifestBuilder(mockRegistrar.Object);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowException()
        {
            mockRegistrar
                .Setup(r => r.IsRegistrationClosed())
                .Returns(true);

            Assert.ThrowsException<ArgumentNullException>(() => new QueryManifestBuilder(null));
            Assert.ThrowsException<ArgumentException>(() => new QueryManifestBuilder(mockRegistrar.Object));
        }

        [TestMethod]
        public void AddQueryHandler_ShouldRegister_AndAddMap()
        {
            // arrange
            mockRegistrar
                .Setup(r => r.Register<Query1Handler>(
                    It.IsAny<RegistryScope>(),
                    It.IsAny<InterceptorProfile>(),
                    It.IsAny<IBindContext[]>()))
                .Returns(mockRegistrar.Object)
                .Verifiable();

            var builder = new QueryManifestBuilder(mockRegistrar.Object);

            // act
            var returned = builder.AddQueryHandler<Query1, Query1Result, Query1Handler>();

            // assert
            Assert.IsTrue(builder
                .QueryMaps()
                .Any(m => m.queryType == typeof(Query1)
                    && m.queryHandlerType == typeof(Query1Handler)));
            Assert.AreEqual(returned, builder);
            mockRegistrar.Verify();
        }

        [TestMethod]
        public void ValidateHandlerMap_ShouldValidate()
        {
            // valid args
            QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair(typeof(Query1Handler)),
                ns => false);

            #region query
            // null predicate
            Assert.ThrowsException<ArgumentNullException>(() => QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair(typeof(Query1Handler)),
                null));

            // null-query type
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                ((Type)null).ValuePair(typeof(Query1Handler)),
                ns => false));

            // non-decorated query type
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                (typeof(NonDecoratedQuery)).ValuePair(typeof(Query1Handler)),
                ns => false));

            // doesn't implement the IQuery interface
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                (typeof(object)).ValuePair(typeof(Query1Handler)),
                ns => false));

            // non-unique namespace query type
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                (typeof(Query1)).ValuePair(typeof(Query1Handler)),
                ns => true));
            #endregion

            #region query handler
            // null handler-type
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair((Type)null),
                ns => false));

            // doesn't implement the IQueryHandler<> interface
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair(typeof(object)),
                ns => false));

            // doesn't implement the IQueryHandler<> interface
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair(typeof(Query2Handler)),
                ns => false));

            // is not a concrete type
            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair(typeof(IQuery1Handler2)),
                ns => false));

            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair(typeof(Query1Handler2)),
                ns => false));

            Assert.ThrowsException<InvalidOperationException>(() => QueryManifestBuilder.ValidateHandlerMap(
                typeof(Query1).ValuePair(typeof(GenericQuery1Handler3<>)),
                ns => false));
            #endregion
        }
    }

    [TestClass]
    public class QueryManifestTest
    {
        private Mock<IResolverContract> mockResolver = new Moq.Mock<IResolverContract>();

        [TestMethod]
        public void Constructor_ShouldReturnValidInstance()
        {
            var manifest = new QueryManifest(
                mockResolver.Object,
                new Dictionary<Type, Type>());
            Assert.IsNotNull(manifest);

            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Query1)] = typeof(Query1Handler)
            };
            manifest = new QueryManifest(mockResolver.Object, handlerMap);
            Assert.IsNotNull(manifest);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowException()
        {
            var handlerMap = new Dictionary<Type, Type>();

            Assert.ThrowsException<ArgumentNullException>(() => new QueryManifest(null, handlerMap));
            Assert.ThrowsException<ArgumentNullException>(() => new QueryManifest(mockResolver.Object, null));

            handlerMap[typeof(Query1)] = typeof(Query1Handler);
            handlerMap[typeof(Query1)] = typeof(Query2Handler);

            Assert.ThrowsException<InvalidOperationException>(() => new QueryManifest(mockResolver.Object, handlerMap));
        }

        [TestMethod]
        public void Namespaces_ShouldReturnAllQueryNamespaces()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Query1)] = typeof(Query1Handler),
                [typeof(Query2)] = typeof(Query2Handler)
            };
            var manifest = new QueryManifest(mockResolver.Object, handlerMap);

            Assert.IsTrue(
                manifest
                    .Namespaces()
                    .SequenceEqual(new[] { typeof(Query1).InstructionNamespace(), typeof(Query2).InstructionNamespace() }));
        }

        [TestMethod]
        public void QueryTypes_ShouldReturnAllCommanTypes()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Query1)] = typeof(Query1Handler),
                [typeof(Query2)] = typeof(Query2Handler)
            };
            var manifest = new QueryManifest(mockResolver.Object, handlerMap);

            Assert.IsTrue(
                manifest
                    .QueryTypes()
                    .SequenceEqual(new[] { typeof(Query1), typeof(Query2) }));
        }

        [TestMethod]
        public void QueryTypeFor_ShouldReturnCorrectQueryType()
        {
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Query1)] = typeof(Query1Handler),
                [typeof(Query2)] = typeof(Query2Handler)
            };
            var manifest = new QueryManifest(mockResolver.Object, handlerMap);

            var queryType = manifest.QueryTypeFor(typeof(Query1).InstructionNamespace());
            Assert.AreEqual(typeof(Query1), queryType);
        }

        [TestMethod]
        public void HandlerFor_ShouldReturnCorrectHandlerInstance()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(new Query1Handler());
            var handlerMap = new Dictionary<Type, Type>
            {
                [typeof(Query1)] = typeof(Query1Handler),
                [typeof(Query2)] = typeof(Query2Handler)
            };
            var manifest = new QueryManifest(mockResolver.Object, handlerMap);

            var handlerInstance = manifest.HandlerFor<Query1, Query1Result>();
            Assert.IsNotNull(handlerInstance);
        }
    }

    public class NonDecoratedQuery : IQuery<int>
    {
        public InstructionURI InstructionURI => default;
    }

    public interface IQuery1Handler2 : IQueryHandler<Query1, Query1Result>
    { }

    public abstract class Query1Handler2 : Query1Handler
    { }

    public class GenericQuery1Handler3<T> : IQueryHandler<Query1, Query1Result>
    {
        public Task<Luna.Common.IResult<Query1Result>> ExecuteQuery(Query1 query)
        {
            throw new NotImplementedException();
        }
    }
}
