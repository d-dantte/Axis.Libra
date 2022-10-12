using Axis.Libra.URI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Axis.Libra.Tests.Unit.URI
{
    [TestClass]
    public class InstructionNamespaceTests
    {
        [TestMethod]
        public void Constructor_WithValidArgs_ShouldCreateInstance()
        {
            var inamespace = new InstructionNamespace("abc");
            Assert.AreNotEqual(default, inamespace);

            inamespace = new InstructionNamespace("abc_xyz");
            Assert.AreNotEqual(default, inamespace);

            inamespace = new InstructionNamespace("abc-xyz");
            Assert.AreNotEqual(default, inamespace);

            inamespace = new InstructionNamespace("abc:xyz:123");
            Assert.AreNotEqual(default, inamespace);

            inamespace = default;
            Assert.IsNull(inamespace.Name);
        }

        [TestMethod]
        public void Constructor_WithInvalidArgs_ShouldThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InstructionNamespace(null));
            Assert.ThrowsException<ArgumentException>(() => new InstructionNamespace(""));
            Assert.ThrowsException<ArgumentException>(() => new InstructionNamespace("1"));
            Assert.ThrowsException<ArgumentException>(() => new InstructionNamespace("*"));
            Assert.ThrowsException<ArgumentException>(() => new InstructionNamespace("^%$%^&"));
            Assert.ThrowsException<ArgumentException>(() => new InstructionNamespace(":abc"));
            Assert.ThrowsException<ArgumentException>(() => new InstructionNamespace("1abc-something"));
            Assert.ThrowsException<ArgumentException>(() => new InstructionNamespace("me:you:they:them::us"));
        }

        [TestMethod]
        public void Name_ShouldContainSuppliedName()
        {
            var name = "name:space:is-here";
            var inamespace = new InstructionNamespace(name);
            Assert.AreEqual(name, inamespace.Name);
        }

        [TestMethod]
        public void ToString_ShouldContainSuppliedName()
        {
            var name = "name:space:is-here";
            var inamespace = new InstructionNamespace(name);
            Assert.AreEqual(name, inamespace.ToString());
        }

        [TestMethod]
        public void ImplicitConversion_ShouldConvertFromString()
        {
            var name = "the:name-space";
            InstructionNamespace @namespace = name;
            Assert.AreEqual(name, @namespace.Name);
        }

        [TestMethod]
        public void Equality_Tests()
        {
            var namespace1 = new InstructionNamespace("the:namespace");
            var namespace2 = new InstructionNamespace("the:other:namespace");
            var namespace3 = new InstructionNamespace("the:namespace");

            Assert.IsTrue(namespace1 == namespace1);
            Assert.IsTrue(namespace1.Equals(namespace1));
            Assert.AreEqual(namespace1, namespace1);
            Assert.AreEqual(namespace1, namespace3);

            Assert.IsFalse(namespace1 == namespace2);
            Assert.IsFalse(namespace2 == namespace1);
            Assert.IsFalse(namespace1.Equals(namespace2));
            Assert.IsFalse(namespace2.Equals(namespace1));
            Assert.AreNotEqual(namespace1, namespace2);
        }
    }
}
