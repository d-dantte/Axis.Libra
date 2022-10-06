using Axis.Libra.Exceptions;
using Axis.Libra.Query;
using Axis.Luna.Operation;
using Axis.Proteus.IoC;
using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Query
{
    using Registration = ServiceRegistrar.RegistrationMap;

    [TestClass]
    public class QueryDispatcherTests
    {
        [TestMethod]
        public async Task Dispatch_WithRegisteredQuery_ShouldCallHandler()
        {
            //// setup
            //var _mockHandler = new Mock<IQueryHandler<SomeQuery, SomeQueryResult>>();
            //_mockHandler
            //    .Setup(h => h.ExecuteQuery(It.IsAny<SomeQuery>()))
            //    .Returns((SomeQuery arg1) => Operation.FromResult(new SomeQueryResult { QueryURI = arg1.QueryURI }))
            //    .Verifiable();

            //var _mockResolver = new Mock<ServiceResolver>(
            //    new Mock<IResolverContract>().Object,
            //    new Mock<IProxyGenerator>().Object,
            //    Array.Empty<Registration>());

            //_mockResolver
            //    .Setup(r => r.Resolve<IQueryHandler<SomeQuery, SomeQueryResult>>())
            //    .Returns(_mockHandler.Object)
            //    .Verifiable();

            //var dispatcher = new QueryDispatcher(_mockResolver.Object);

            //// test
            //var query = new SomeQuery { Arg1 = "1", Arg2 = "2" };
            //var op = dispatcher.Dispatch<SomeQuery, SomeQueryResult>(query);
            //var queryResult = await op;

            //// assert
            //_mockResolver.Verify();
            //_mockHandler.Verify();
            //Assert.AreEqual(query.QueryURI, queryResult.QueryURI);
        }

        [TestMethod]
        public async Task Dispatch_WithNullQuery_ShouldThrowException()
        {
            //// setup
            //var _mockResolver = new Mock<ServiceResolver>(
            //    new Mock<IResolverContract>().Object,
            //    new Mock<IProxyGenerator>().Object,
            //    Array.Empty<Registration>());
            //var dispatcher = new QueryDispatcher(_mockResolver.Object);

            //// test
            //var op = dispatcher.Dispatch<SomeQuery, SomeQueryResult>(null);

            //// assert
            //await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await op);
        }

        [TestMethod]
        public async Task Dispatch_WithUnregisteredQuery_ShouldThrowException()
        {
            //// setup
            //var _mockHandler = new Mock<IQueryHandler<SomeQuery, SomeQueryResult>>();
            //var _mockResolver = new Mock<ServiceResolver>(
            //    new Mock<IResolverContract>().Object,
            //    new Mock<IProxyGenerator>().Object,
            //    Array.Empty<Registration>());

            //_mockResolver
            //    .Setup(r => r.Resolve<IQueryHandler<SomeQuery, SomeQueryResult>>())
            //    .Throws(new SimpleInjector.ActivationException())
            //    .Verifiable();

            //var dispatcher = new QueryDispatcher(_mockResolver.Object);

            //// test
            //var query = new SomeQuery { Arg1 = "1", Arg2 = "2" };
            //var op = dispatcher.Dispatch<SomeQuery, SomeQueryResult>(query);

            //// assert
            //await Assert.ThrowsExceptionAsync<SimpleInjector.ActivationException>(async () => await op);
            //_mockResolver.Verify();
            //_mockHandler.Verify();
        }

        [TestMethod]
        public async Task Dispatch_WhenResolverYieldsNull_ShouldThrowException()
        {
            //// setup
            //var _mockHandler = new Mock<IQueryHandler<SomeQuery, SomeQueryResult>>();
            //var _mockResolver = new Mock<ServiceResolver>(
            //    new Mock<IResolverContract>().Object,
            //    new Mock<IProxyGenerator>().Object,
            //    Array.Empty<Registration>());

            //_mockResolver
            //    .Setup(r => r.Resolve<IQueryHandler<SomeQuery, SomeQueryResult>>())
            //    .Returns<SomeQuery>(null)
            //    .Verifiable();

            //var dispatcher = new QueryDispatcher(_mockResolver.Object);

            //// test
            //var query = new SomeQuery { Arg1 = "1", Arg2 = "2" };
            //var op = dispatcher.Dispatch<SomeQuery, SomeQueryResult>(query);

            //// assert
            //await Assert.ThrowsExceptionAsync<UnknownResolverException>(async () => await op);
            //_mockResolver.Verify();
            //_mockHandler.Verify();
        }
    }
}
