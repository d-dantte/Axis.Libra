using Axis.Libra.Command;
using Axis.Libra.URI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Axis.Libra.Tests.Unit.Command
{
    [TestClass]
    public class CommandStatusTests
    {
        #region Success
        [TestMethod]
        public void Success_Constructor_ShouldReturnValidInstance()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Succeeded(uri);

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void Success_Constructor_WithInvalidArg_ShouldThrowException()
        {
            var uri = default(InstructionURI);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Succeeded(uri));

            uri = new InstructionURI(
                Scheme.Request,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Succeeded(uri));

            uri = new InstructionURI(
                Scheme.Query,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Succeeded(uri));
        }

        [TestMethod]
        public void Success_CommandURI_ShouldReturnSuppliedValue()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Succeeded(uri);
            Assert.AreEqual(uri, status.CommandURI);
        }

        [TestMethod]
        public void Success_ToString_ReturnsProperString()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Succeeded(uri);
            var expectedString = $"{nameof(ICommandStatus.Succeeded)}[{uri}]";
            Assert.AreEqual(expectedString, status.ToString());
        }
        #endregion

        #region Busy
        [TestMethod]
        public void Busy_Constructor_ShouldReturnValidInstance()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Busy(uri);

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void Busy_Constructor_WithInvalidArg_ShouldThrowException()
        {
            var uri = default(InstructionURI);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Busy(uri));

            uri = new InstructionURI(
                Scheme.Request,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Busy(uri));

            uri = new InstructionURI(
                Scheme.Query,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Busy(uri));
        }

        [TestMethod]
        public void Busy_CommandURI_ShouldReturnSuppliedValue()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Busy(uri);
            Assert.AreEqual(uri, status.CommandURI);
        }

        [TestMethod]
        public void Busy_ToString_ReturnsProperString()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Busy(uri);
            var expectedString = $"{nameof(ICommandStatus.Busy)}[{uri}]";
            Assert.AreEqual(expectedString, status.ToString());
        }
        #endregion

        #region Unknown
        [TestMethod]
        public void Unknown_Constructor_ShouldReturnValidInstance()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Unknown(uri);

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void Unknown_Constructor_WithInvalidArg_ShouldThrowException()
        {
            var uri = default(InstructionURI);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Unknown(uri));

            uri = new InstructionURI(
                Scheme.Request,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Unknown(uri));

            uri = new InstructionURI(
                Scheme.Query,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Unknown(uri));
        }

        [TestMethod]
        public void Unknown_CommandURI_ShouldReturnSuppliedValue()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Unknown(uri);
            Assert.AreEqual(uri, status.CommandURI);
        }

        [TestMethod]
        public void Unknown_ToString_ReturnsProperString()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Unknown(uri);
            var expectedString = $"{nameof(ICommandStatus.Unknown)}[{uri}]";
            Assert.AreEqual(expectedString, status.ToString());
        }
        #endregion

        #region Progress
        [TestMethod]
        public void Progress_Constructor_ShouldReturnValidInstance()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Progress(uri, 67.8m);

            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void Progress_Constructor_WithInvalidArg_ShouldThrowException()
        {
            var uri = default(InstructionURI);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Progress(uri, 0m));

            uri = new InstructionURI(
                Scheme.Request,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Progress(uri, 0m));

            uri = new InstructionURI(
                Scheme.Query,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Progress(uri, 0m));

            uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Progress(uri, -10m));
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Progress(uri, 11110m));
        }

        [TestMethod]
        public void Progress_CommandURI_ShouldReturnSuppliedValue()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Progress(uri, 0m);
            Assert.AreEqual(uri, status.CommandURI);
        }

        [TestMethod]
        public void Progress_Progress_ShouldReturnSuppliedValue()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var progress = 34.75m;
            var status = new ICommandStatus.Progress(uri, progress);
            Assert.AreEqual(progress, status.Percentage);
        }

        [TestMethod]
        public void Progress_ToString_ReturnsProperString()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var progress = 100m;
            var status = new ICommandStatus.Progress(uri, progress);
            var expectedString = $"{progress}% [{uri}]";
            Assert.AreEqual(expectedString, status.ToString());
        }
        #endregion

        #region Error
        [TestMethod]
        public void Error_Constructor_ShouldReturnValidInstance()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Error(uri, "some error has occured");
            Assert.IsNotNull(status);

            status = new ICommandStatus.Error(uri, null);
            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void Error_Constructor_WithInvalidArg_ShouldThrowException()
        {
            var uri = default(InstructionURI);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Error(uri, null));

            uri = new InstructionURI(
                Scheme.Request,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Error(uri, null));

            uri = new InstructionURI(
                Scheme.Query,
                "namespace",
                0ul);
            Assert.ThrowsException<ArgumentException>(() => new ICommandStatus.Error(uri, null));
        }

        [TestMethod]
        public void Error_CommandURI_ShouldReturnSuppliedValue()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var status = new ICommandStatus.Error(uri, null);
            Assert.AreEqual(uri, status.CommandURI);
        }

        [TestMethod]
        public void Error_Error_ShouldReturnSuppliedValue()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var message = "the error message";
            var status = new ICommandStatus.Error(uri, message);
            Assert.AreEqual(message, status.Message);
        }

        [TestMethod]
        public void Error_ToString_ReturnsProperString()
        {
            var uri = new InstructionURI(
                Scheme.Command,
                "namespace",
                0ul);

            var message = "the error message";
            var status = new ICommandStatus.Error(uri, message);
            var expectedString = $"{nameof(ICommandStatus.Error)}[{uri}]: {message}";
            Assert.AreEqual(expectedString, status.ToString());

            message = "   \n\r\t";
            status = new ICommandStatus.Error(uri, message);
            expectedString = $"{nameof(ICommandStatus.Error)}[{uri}]: {message}";
            Assert.AreEqual(expectedString, status.ToString());

            message = null;
            status = new ICommandStatus.Error(uri, message);
            expectedString = $"{nameof(ICommandStatus.Error)}[{uri}]";
            Assert.AreEqual(expectedString, status.ToString());

            message = "";
            status = new ICommandStatus.Error(uri, message);
            expectedString = $"{nameof(ICommandStatus.Error)}[{uri}]";
            Assert.AreEqual(expectedString, status.ToString());
        }
        #endregion
    }
}
