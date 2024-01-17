using Axis.Libra.Request;
using Axis.Libra.Instruction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Axis.Libra.Tests.TestCQRs.Requests;
using Axis.Libra.Tests.TestCQRs.Requests.Inner;

namespace Axis.Libra.Tests.Unit.Request
{
    [TestClass]
    public class RequestManifestBuilderTests
    {

        [TestMethod]
        public void AddRequestHandler_ShouldRegister_AndAddMap()
        {
            // arrange
            var builder = new RequestManifestBuilder();

            // act
            var returnedBuilder1 = builder.AddRequestHandler<Request1, Request1Handler, Request1Result>(
                (qry, uri, handler) => handler.ExecuteRequest(qry));

            var returnedBuilder2 = builder.AddRequestHandler<Request2, Request2Handler, Request2Result>(
                (qry, uri, handler) => handler.ExecuteRequest(qry),
                (qry) => unchecked((ulong)qry.Something.GetHashCode()),
                new InstructionNamespace("Request2"));

            // test

            // 1. reject duplicates
            Assert.ThrowsException<InvalidOperationException>(() =>
                builder.AddRequestHandler<Request1, Request1Handler, Request1Result>(
                    (qry, uri, handler) => handler.ExecuteRequest(qry)));

            // 2. returned builders are same as original builder
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder1));
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder2));

            // 3. builder has 2 elements
            Assert.AreEqual(2, builder.RequestInfoList.Length);
        }
    }

    [TestClass]
    public class RequestManifestTest
    {
        [TestMethod]
        public void Constructor_ShouldReturnValidInstance()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RequestManifest(null));
            Assert.ThrowsException<ArgumentException>(() => new RequestManifest(
                new[] { default(RequestInfo) }));
        }

        [TestMethod]
        public void Namespaces_ShouldReturnAllRequestNamespaces()
        {
            var manifest = new RequestManifest(GetRequestInfoList());
            var namespaces = manifest.Namespaces();

            Assert.AreEqual(3, namespaces.Length);
        }

        [TestMethod]
        public void RequestTypes_ShouldReturnAllCommanTypes()
        {
            var manifest = new RequestManifest(GetRequestInfoList());
            var types = manifest.RequestTypes();

            Assert.AreEqual(3, types.Length);
        }

        [TestMethod]
        public void GetRequestInfo_ShouldReturnCorrectInfo()
        {
            var manifest = new RequestManifest(GetRequestInfoList());

            var info = manifest.GetRequestInfo<Request1>();
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(Request1), info.Value.RequestType);

            info = manifest.GetRequestInfo("Request1.Namespace");
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(Request1), info.Value.RequestType);
            Assert.AreEqual(new InstructionNamespace("Request1.Namespace"), info.Value.Namespace);
            Assert.AreEqual(typeof(Request1Handler), info.Value.HandlerType);
        }

        private ImmutableArray<RequestInfo> GetRequestInfoList()
        {
            return ImmutableArray.Create(
                new RequestInfo(
                    new InstructionNamespace("Request1.Namespace"),
                    typeof(Request1),
                    typeof(Request1Handler),
                    (x, y, z) => Task.CompletedTask,
                    (x) => 0),
                new RequestInfo(
                    new InstructionNamespace("Request2.Namespace"),
                    typeof(Request2),
                    typeof(Request2Handler),
                    (x, y, z) => Task.CompletedTask,
                    (x) => 0),
                new RequestInfo(
                    new InstructionNamespace("Request3.Namespace"),
                    typeof(Request3),
                    typeof(Request3Handler),
                    (x, y, z) => Task.CompletedTask,
                    (x) => 0));
        }
    }
}
