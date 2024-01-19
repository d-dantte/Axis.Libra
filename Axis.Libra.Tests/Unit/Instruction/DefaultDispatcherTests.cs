using Axis.Libra.Command;
using Axis.Libra.Event;
using Axis.Libra.Exceptions;
using Axis.Libra.Instruction;
using Axis.Libra.Query;
using Axis.Libra.Request;
using Axis.Libra.Tests.TestCQRs.Commands;
using Axis.Libra.Tests.TestCQRs.Commands.Inner;
using Axis.Libra.Tests.TestCQRs.Events;
using Axis.Libra.Tests.TestCQRs.Queries;
using Axis.Libra.Tests.TestCQRs.Queries.Inner;
using Axis.Libra.Tests.TestCQRs.Requests;
using Axis.Libra.Tests.TestCQRs.Requests.Inner;
using Axis.Libra.Tests.Unit.Command;
using Axis.Libra.Tests.Unit.Event;
using Axis.Libra.Tests.Unit.Query;
using Axis.Libra.Tests.Unit.Request;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Proteus.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Instruction
{
    [TestClass]
    public class DefaultDispatcherTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            var resolver = SetupResolver(null, null);
            var cmanifest = CreateCommandManifest();
            var qmanifest = CreateQueryManifest();
            var rmanifest = CreateRequestManifest();
            var emanifest = CreateEventManifest();

            Assert.ThrowsException<ArgumentNullException>(() => new DefaultDispatcher(
                null, cmanifest, qmanifest, rmanifest, emanifest));

            Assert.ThrowsException<ArgumentNullException>(() => new DefaultDispatcher(
                resolver, null, qmanifest, rmanifest, emanifest));

            Assert.ThrowsException<ArgumentNullException>(() => new DefaultDispatcher(
                resolver, cmanifest, null, rmanifest, emanifest));

            Assert.ThrowsException<ArgumentNullException>(() => new DefaultDispatcher(
                resolver, cmanifest, qmanifest, null, emanifest));

            Assert.ThrowsException<ArgumentNullException>(() => new DefaultDispatcher(
                resolver, cmanifest, qmanifest, rmanifest, null));

            Assert.IsNotNull(new DefaultDispatcher(
                resolver, cmanifest, qmanifest, rmanifest, emanifest));
        }

        #region Command
        [TestMethod]
        public void DispatchCommand_Tests()
        {
            var commandCount = 0;

            var dispatcher = CreateDispatcher(
                () => commandCount++,
                null);

            Assert.ThrowsException<ArgumentNullException>(
                () => dispatcher.DispatchCommand<Command1>(null));

            Assert.ThrowsException<UnregisteredInstructionException>(
                () => dispatcher.DispatchCommand(new object()));

            Assert.ThrowsException<UnresolvedHandlerException>(
                () => dispatcher.DispatchCommand(new Command3()));

            var uri = dispatcher.DispatchCommand(new Command1());
            Thread.Sleep(50);
            Assert.AreEqual(Command1.InstructionNamespace(), uri.Namespace);
            Assert.AreEqual(1, commandCount);
        }

        [TestMethod]
        public async Task DispatchCommandStatusQuery_Tests()
        {
            var queryCount = 0;

            var dispatcher = CreateDispatcher(
                () => queryCount++,
                null);

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => dispatcher.DispatchCommandStatusQuery(default));

            await Assert.ThrowsExceptionAsync<UnregisteredNamespaceException>(
                () => dispatcher.DispatchCommandStatusQuery(new InstructionURI(
                    Scheme.Event,
                    "bleh.bleh",
                    34)));

            await Assert.ThrowsExceptionAsync<UnresolvedHandlerException>(
                () => dispatcher.DispatchCommandStatusQuery(
                    new InstructionURI(
                        Scheme.Command,
                        CommandManifestTest.Command3Namespace,
                        45)));

            var status = await dispatcher.DispatchCommandStatusQuery(new InstructionURI(
                Scheme.Command,
                Command1.InstructionNamespace(),
                55));
            Console.WriteLine(status);
            Assert.AreEqual(1, queryCount);
        }
        #endregion

        #region Query
        [TestMethod]
        public async Task DispatchQuery_Tests()
        {
            var dispatcher = CreateDispatcher(null, null);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => dispatcher.DispatchQuery<Query1, Query1Result>(null));

            await Assert.ThrowsExceptionAsync<UnregisteredInstructionException>(
                () => dispatcher.DispatchQuery<object, object>(new object()));

            await Assert.ThrowsExceptionAsync<UnresolvedHandlerException>(
                () => dispatcher.DispatchQuery<Query3, Query3Result>(new Query3()));

            var result = await dispatcher.DispatchQuery<Query1, Query1Result>(new Query1());
            Assert.IsNotNull(result);
        }
        #endregion

        #region Request
        [TestMethod]
        public void DispatchRequest_Tests()
        {
            var dispatcher = CreateDispatcher(null, null);

            Assert.ThrowsException<ArgumentNullException>(
                () => dispatcher.DispatchRequest<Request1, Request1Result>(null));

            Assert.ThrowsException<UnregisteredInstructionException>(
                () => dispatcher.DispatchRequest<object, object>(new object()));

            Assert.ThrowsException<UnresolvedHandlerException>(
                () => dispatcher.DispatchRequest<Request3, Request3Result>(new Request3()));

            var result = dispatcher.DispatchRequest<Request1, Request1Result>(new Request1());
            Assert.IsNotNull(result);
        }
        #endregion

        #region Event
        [TestMethod]
        public void DispatchEvent_Tests()
        {
            var eventCount = 0;

            var dispatcher = CreateDispatcher(
                null,
                () => eventCount++);

            Assert.ThrowsException<ArgumentNullException>(
                () => dispatcher.BroadcastEvent<Event1>(null));

            Assert.ThrowsException<UnregisteredInstructionException>(
                () => dispatcher.BroadcastEvent(new object()));

            var aggregateException = Assert.ThrowsException<AggregateException>(
                () => dispatcher.BroadcastEvent(new Event2()));
            Assert.AreEqual(1, aggregateException.InnerExceptions.Count(ex => ex is UnresolvedHandlerException));

            var tsource = dispatcher.BroadcastEvent(new Event1());
            Thread.Sleep(50);
            Assert.AreEqual(2, eventCount);
        }
        #endregion

        private static DefaultDispatcher CreateDispatcher(
            Action commandCallback,
            Action eventCallback)
        {
            return new DefaultDispatcher(
                SetupResolver(commandCallback, eventCallback),
                CreateCommandManifest(),
                CreateQueryManifest(),
                CreateRequestManifest(),
                CreateEventManifest());
        }

        private static IResolverContract SetupResolver(
            Action commandCallback,
            Action eventCallback)
            => new MockResolver(
                new Command1Handler(commandCallback),
                new Command2Handler(),
                //new Command3Handler(),
            
                new Query1Handler(),
                new Query2Handler(),
                //new Query3Handler(),
            
                new Request1Handler(),
                new Request2Handler(),
                //new Request3Handler(),

                new Event1Handler_1(eventCallback),
                new Event1Handler_2(eventCallback)
                //,new Event2Handler_1()
                );

        private static CommandManifest CreateCommandManifest()
        {
            return new CommandManifest(CommandManifestTest.GetCommandInfoList());
        }

        private static QueryManifest CreateQueryManifest()
        {
            return new QueryManifest(QueryManifestTest.GetQueryInfoList());
        }

        private static RequestManifest CreateRequestManifest()
        {
            return new RequestManifest(RequestManifestTest.GetRequestInfoList());
        }

        private static EventManifest CreateEventManifest()
        {
            return new EventManifest(
                EventManifestTests.GetOptions(),
                EventManifestTests.GetEventInfoList());
        }


        internal class MockResolver : IResolverContract
        {
            private readonly Dictionary<Type, object> _objects = new();

            public MockResolver(params object[] handlers)
            {
                handlers.ForAll(handler => _objects[handler.GetType()] = handler);
            }

            public ReadOnlyDictionary<Type, RegistrationInfo[]> CollectionManifest()
                => new(new Dictionary<Type, RegistrationInfo[]>());

            public void Dispose() { }

            public Service Resolve<Service>(ResolutionContextName contextName = default) where Service : class
            {
                _ = _objects.TryGetValue(typeof(Service), out var v);
                return (Service)v;
            }

            public object Resolve(Type serviceType, ResolutionContextName contextName = default)
            {
                _ = _objects.TryGetValue(serviceType, out var v);
                return v;
            }

            public IEnumerable<Service> ResolveAll<Service>() where Service : class
            {
                return ArrayUtil.Of(Resolve<Service>());
            }

            public IEnumerable<object> ResolveAll(Type serviceType)
            {
                return ArrayUtil.Of(Resolve(serviceType));
            }

            public ReadOnlyDictionary<Type, RegistrationInfo> RootManifest() => new(
                new Dictionary<Type, RegistrationInfo>());
        }
    }
}
