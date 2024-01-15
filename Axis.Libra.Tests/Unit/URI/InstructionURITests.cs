using Axis.Libra.Instruction;
using Axis.Luna.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.URI
{
    [TestClass]
    public class InstructionURITests
    {
        [TestMethod]
        public void Constructor_WithValidArgs_ShouldReturnInstance()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var @namespace = "axis:libra:stuff";
            var rscheme = Scheme.Request;
            var cscheme = Scheme.Command;
            var qscheme = Scheme.Query;
            var idbytes = new byte[random.Next(100)+2];
            var ulongBytes = new byte[8];

            var uri = new InstructionURI(
                rscheme,
                @namespace,
                idbytes.Use(random.NextBytes));

            Assert.AreNotEqual(default, uri);
            Console.WriteLine(uri);

            uri = new InstructionURI(
                cscheme,
                @namespace,
                BitConverter.ToUInt64(ulongBytes.Use(random.NextBytes)));

            Assert.AreNotEqual(default, uri);
            Console.WriteLine(uri);

            uri = new InstructionURI(
                qscheme,
                @namespace,
                "89E-F1BC-9518F189");

            Assert.AreNotEqual(default, uri);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowExeption()
        {
            var random = new Random(DateTime.Now.Millisecond);

            // ctor(Schema, InstructionNamespace, byte[]
            Assert.ThrowsException<ArgumentNullException>(() => new InstructionURI(
                Scheme.Command,
                "some:namespace",
                (byte[])null));

            Assert.ThrowsException<ArgumentException>(() => new InstructionURI(
                Scheme.Command,
                default,
                new byte[100].Use(random.NextBytes)));

            // ctor(Schema, InstructionNamespace, ulong)

            Assert.ThrowsException<ArgumentException>(() => new InstructionURI(
                Scheme.Command,
                default,
                new byte[100].Use(random.NextBytes)));

            // ctor(Schema, InstructionNamespace, string)

            Assert.ThrowsException<ArgumentException>(() => new InstructionURI(
                Scheme.Command,
                default,
                "00-00-0000"));

            Assert.ThrowsException<ArgumentNullException>(() => new InstructionURI(
                Scheme.Command,
                "valid:namespace",
                (string)null));

            Assert.ThrowsException<ArgumentException>(() => new InstructionURI(
                Scheme.Command,
                default,
                "invalid format"));


        }

        [TestMethod]
        public void ToString_Tests()
        {
            Console.WriteLine(100ul.ToSignatureHashString());
            var uri = new InstructionURI(Scheme.Query, "the-namespace:to:end:all:name-spaces", 100ul);
            var uriString = uri.ToString();
            Assert.AreEqual("qry:the-namespace:to:end:all:name-spaces/640-00-0000", uriString);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var @namespace = "axis:libra:stuff";
            var idbytes = new byte[random.Next(100) + 2];
            var ulongBytes = new byte[8];

            var uri = new InstructionURI(
                Scheme.Query,
                @namespace,
                idbytes.Use(random.NextBytes));
            var uriString = uri.ToString();
            var parsed = InstructionURI.Parse(uriString);
            Assert.AreEqual(uri, parsed);

            uri = new InstructionURI(
                Scheme.Request,
                @namespace,
                BitConverter.ToUInt64(ulongBytes.Use(random.NextBytes)));
            uriString = uri.ToString();
            parsed = InstructionURI.Parse(uriString);
            Assert.AreEqual(uri, parsed);

            uri = new InstructionURI(
                Scheme.Command,
                @namespace,
                "89E-F1BC-9518F189");
            uriString = uri.ToString();
            parsed = InstructionURI.Parse(uriString);
            Assert.AreEqual(uri, parsed);

            uri = default;
            uriString = uri.ToString();
            parsed = InstructionURI.Parse(uriString);
            Assert.AreEqual("*", uriString);
            Assert.AreEqual(default, parsed);
        }

        [TestMethod]
        public void TryParse_Tests()
        {
            var validUri = "req:axis:libra:arbil/7865-1181-4DCE1534";
            var invalidUri = "ble:namespace:namespce/abcdefghigk";

            Assert.IsTrue(InstructionURI.TryParse(validUri, out var uri));
            Assert.AreEqual(validUri, uri.ToString());

            Assert.IsFalse(InstructionURI.TryParse(invalidUri, out uri));
        }

        [TestMethod]
        public void ImplicitOperator_Tests()
        {
            var uriString = "req:axis:libra:stuff/7865-1181-4DCE1534";
            InstructionURI uri = uriString;
            Assert.AreEqual(uriString, uri.ToString());
        }
    }
}
