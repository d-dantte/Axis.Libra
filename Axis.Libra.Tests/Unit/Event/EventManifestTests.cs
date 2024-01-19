using Axis.Libra.Event;
using Axis.Libra.Tests.TestCQRs.Events;
using Axis.Luna.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Event
{
    [TestClass]
    public class EventManifestBuilderTests
    {

        [TestMethod]
        public void AddEventHandler_ShouldRegister_AndAddMap()
        {
            // arrange
            var builder = new EventManifestBuilder();

            // act
            var returnedBuilder1 = builder.AddEventHandler<Event1, Event1Handler_1>();
            var returnedBuilder2 = builder.AddEventHandler<Event1, Event1Handler_2>();

            // test

            // 1. reject duplicates
            Assert.ThrowsException<InvalidOperationException>(() =>
                builder.AddEventHandler<Event1, Event1Handler_1>());

            // 2. returned builders are same as original builder
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder1));
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder2));

            // 3. builder has 2 handlers
            Assert.AreEqual(2, builder.RegisteredHandlers);

            // 4. builder has 1 event
            Assert.AreEqual(1, builder.RegisteredEvents);
        }
    }

    [TestClass]
    public class EventManifestTests
    {
        [TestMethod]
        public void Constructor_ShouldReturnValidInstance()
        {
            var options = GetOptions();

            Assert.ThrowsException<ArgumentException>(() => new EventManifest(
                default,
                null));

            Assert.ThrowsException<ArgumentNullException>(() => new EventManifest(
                options,
                null));

            Assert.ThrowsException<ArgumentException>(() => new EventManifest(
                options,
                new[] { default(EventInfo)}));

            Assert.IsNotNull(new EventManifest(options, GetEventInfoList()));
        }

        [TestMethod]
        public void EventTypes_ShouldReturnAllCommanTypes()
        {
            var options = GetOptions();

            var manifest = new EventManifest(options, GetEventInfoList());
            var types = manifest.EventTypes();

            Assert.AreEqual(2, types.Length);
        }

        [TestMethod]
        public void GetEventInfo_ShouldReturnCorrectInfo()
        {
            var options = GetOptions();

            var manifest = new EventManifest(options, GetEventInfoList());

            var info = manifest.GetEventInfo<Event1>();
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(Event1), info.Value.EventType);
            Assert.AreEqual(2, info.Value.HandlerTypes.Length);
        }

        internal static Options GetOptions() =>new Options(
            new CancellationTokenSource(),
            TaskScheduler.Current,
            TaskCreationOptions.None);

        internal static ImmutableArray<EventInfo> GetEventInfoList()
        {
            return ImmutableArray.Create(
                new EventInfo(
                    typeof(Event1),
                    ArrayUtil.Of(
                        typeof(Event1Handler_1),
                        typeof(Event1Handler_2))),
                new EventInfo(
                    typeof(Event2),
                    ArrayUtil.Of(
                        typeof(Event2Handler_1)/*,
                        typeof(Event2Handler_2)*/)));
        }
    }
}
