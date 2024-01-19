using Axis.Libra.Query;
using Axis.Libra.Tests.TestCQRs.Queries;
using Axis.Libra.Instruction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Axis.Libra.Tests.TestCQRs.Queries.Inner;

namespace Axis.Libra.Tests.Unit.Query
{
    [TestClass]
    public class QueryManifestBuilderTests
    {

        [TestMethod]
        public void AddQueryHandler_ShouldRegister_AndAddMap()
        {
            // arrange
            var builder = new QueryManifestBuilder();

            // act
            var returnedBuilder1 = builder.AddQueryHandler<Query1, Query1Handler, Query1Result>(
                (qry, uri, handler) => handler.ExecuteQuery(qry));

            var returnedBuilder2 = builder.AddQueryHandler<Query2, Query2Handler, Query2Result>(
                (qry, uri, handler) => handler.ExecuteQuery(qry),
                (qry) => unchecked((ulong)qry.Something.GetHashCode()),
                new InstructionNamespace("Query2"));

            // test

            // 1. reject duplicates
            Assert.ThrowsException<InvalidOperationException>(() =>
                builder.AddQueryHandler<Query1, Query1Handler, Query1Result>(
                    (qry, uri, handler) => handler.ExecuteQuery(qry)));

            // 2. returned builders are same as original builder
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder1));
            Assert.IsTrue(object.ReferenceEquals(builder, returnedBuilder2));

            // 3. builder has 2 elements
            Assert.AreEqual(2, builder.QueryInfoList.Length);
        }
    }

    [TestClass]
    public class QueryManifestTest
    {
        [TestMethod]
        public void Constructor_ShouldReturnValidInstance()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryManifest(null));
            Assert.ThrowsException<ArgumentException>(() => new QueryManifest(
                new[] { default(QueryInfo) }));
        }

        [TestMethod]
        public void Namespaces_ShouldReturnAllQueryNamespaces()
        {
            var manifest = new QueryManifest(GetQueryInfoList());
            var namespaces = manifest.Namespaces();

            Assert.AreEqual(3, namespaces.Length);
        }

        [TestMethod]
        public void QueryTypes_ShouldReturnAllCommanTypes()
        {
            var manifest = new QueryManifest(GetQueryInfoList());
            var types = manifest.QueryTypes();

            Assert.AreEqual(3, types.Length);
        }

        [TestMethod]
        public void GetQueryInfo_ShouldReturnCorrectInfo()
        {
            var manifest = new QueryManifest(GetQueryInfoList());

            var info = manifest.GetQueryInfo<Query1>();
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(Query1), info.Value.QueryType);

            info = manifest.GetQueryInfo(Query1.InstructionNamespace());
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(Query1), info.Value.QueryType);
            Assert.AreEqual(Query1.InstructionNamespace(), info.Value.Namespace);
            Assert.AreEqual(typeof(Query1Handler), info.Value.HandlerType);
        }

        internal static ImmutableArray<QueryInfo> GetQueryInfoList()
        {
            return ImmutableArray.Create(
                new QueryInfo(
                    Query1.InstructionNamespace(),
                    typeof(Query1),
                    typeof(Query1Handler),
                    (x, y, z) => ((Query1Handler)z).ExecuteQuery((Query1)x),
                    (x) => 0),
                new QueryInfo(
                    new InstructionNamespace("Query2.Namespace"),
                    typeof(Query2),
                    typeof(Query2Handler),
                    (x, y, z) => ((Query2Handler)z).ExecuteQuery((Query2)x),
                    (x) => 0),
                new QueryInfo(
                    new InstructionNamespace("Query3.Namespace"),
                    typeof(Query3),
                    typeof(Query3Handler),
                    (x, y, z) => ((Query3Handler)z).ExecuteQuery((Query3)x),
                    (x) => 0));
        }
    }
}
