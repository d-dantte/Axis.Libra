using Axis.Libra.Query;
using Axis.Libra.Tests.TestCQRs.Queries;
using Axis.Proteus.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Query
{

    [TestClass]
    public class QueryDispatcherTests
    {
        private Mock<IResolverContract> mockResolver = new Mock<IResolverContract>();

        [TestMethod]
        public async Task DispatchQuery_WithValidArgs_ShouldExecuteQueryHandler()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(new Query1Handler());
            var manifest = new QueryManifest(
                mockResolver.Object,
                new System.Collections.Generic.Dictionary<Type, Type>
                {
                    [typeof(Query1)] = typeof(Query1Handler)
                });
            var dispatcher = new QueryDispatcher(manifest);

            var result = await dispatcher.Dispatch<Query1, Query1Result>(new Query1());
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task DispatchQuery_WithInvalidArgs_ShouldThrowException()
        {
            mockResolver
                .Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<ResolutionContextName>()))
                .Returns(null);
            var manifest = new QueryManifest(
                mockResolver.Object,
                new System.Collections.Generic.Dictionary<Type, Type>
                {
                    [typeof(Query1)] = typeof(Query1Handler)
                });
            var dispatcher = new QueryDispatcher(manifest);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dispatcher.Dispatch<Query1, Query1Result>(null));
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => dispatcher.Dispatch<Query1, Query1Result>(new Query1()));
        }
    }
}
