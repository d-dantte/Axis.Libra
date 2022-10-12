using Axis.Libra.Command;
using Axis.Libra.Tests.TestCQRs.Commands;
using Axis.Proteus.IoC;
using Axis.Proteus.SimpleInjector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Libra.Tests.Functional.Command
{
    [TestClass]
    public class CommandRegistrarTests
    {
        //[TestMethod]
        //public void BeginRegistration_WithValidServiceRegistrar_ShouldReturnValidCommandRegistrar()
        //{
        //    // setup
        //    var contract = new Mock<IRegistrarContract>();

        //    // test
        //    var qregistrar = CommandRegistrar.BeginRegistration(contract.Object);

        //    // assert
        //    Assert.IsNotNull(qregistrar);
        //}


        //#region AddHandlerRegistration<TImpl, TCommand>(RegistryScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddHandlerRegistration_ShouldRegisterGivenType()
        //{
        //    // setup
        //    var contract = new SimpleInjectorRegistrar();
        //    var cregistrar = CommandRegistrar.BeginRegistration(contract);

        //    // test
        //    var cregistrar2 = cregistrar.AddHandlerRegistration<Command1Handler, Command1>();
        //    var manifest = cregistrar.CommitRegistrations();

        //    // assert
        //    Assert.AreEqual(cregistrar, cregistrar2);

        //    var cinfos = manifest.Commands.ToArray();
        //    Assert.AreEqual(1, cinfos.Length);
        //    Assert.AreEqual(typeof(Command1), cinfos[0]);
        //    Assert.AreEqual(typeof(Command1Status), manifest.StatusResultFor(cinfos[0].InstructionNamespace()));

        //    var htypes = manifest.CommandHandlers.ToArray();
        //    Assert.AreEqual(1, htypes.Length);
        //    Assert.AreEqual(typeof(ICommandHandler<Command1>), htypes[0]);
        //}
        //#endregion

        //#region AddHandlerRegistration(Type, RegistrationScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddHandlerRegistration2_WithValidTypes_ShouldRegisterGivenType()
        //{
        //    // setup
        //    var contract = new SimpleInjectorRegistrar();
        //    var cregistrar = CommandRegistrar.BeginRegistration(contract);

        //    //test
        //    var cregistrar2 = cregistrar.AddHandlerRegistration(typeof(Command2Handler));
        //    var manifest = cregistrar.CommitRegistrations();

        //    //assert
        //     Assert.AreEqual(cregistrar, cregistrar2);

        //    var ctypes = manifest.Commands.ToArray();
        //    Assert.AreEqual(1, ctypes.Length);
        //    Assert.AreEqual(typeof(Command2), ctypes[0]);
        //    Assert.AreEqual(typeof(Command2Status), manifest.StatusResultFor(ctypes[0].InstructionNamespace()));

        //    var htypes = manifest.CommandHandlers.ToArray();
        //    Assert.AreEqual(1, htypes.Length);
        //    Assert.AreEqual(typeof(ICommandHandler<Command2>), htypes[0]);
        //}

        //[TestMethod]
        //public void AddHandlerRegistration2_WithInvalidType_ShouldThrowException()
        //{
        //    // setup
        //    var contract = new SimpleInjectorRegistrar();
        //    var cregistrar = CommandRegistrar.BeginRegistration(contract);

        //    // test & assert
        //    Assert.ThrowsException<ArgumentException>(() => cregistrar.AddHandlerRegistration(typeof(TestCQRs.Queries.Query1Handler)));
        //    Assert.ThrowsException<ArgumentException>(() => cregistrar.AddHandlerRegistration(typeof(object)));
        //    Assert.ThrowsException<ArgumentException>(() => cregistrar.AddHandlerRegistration(typeof(IEnumerable<int>)));
        //}
        //#endregion

        //#region AddNamespaceHandlerRegistrations(string, RegistryScope?, InterceptorProfile? bool)
        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithValidArgument_ShouldRegisterFoundHandlers()
        //{
        //    // setup
        //    var contract = new SimpleInjectorRegistrar();
        //    var sregistrar = new ServiceRegistrar(contract);
        //    var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    // test
        //    var cregistrar2 = cregistrar.AddNamespaceHandlerRegistrations(typeof(LocalTypes.LocalCommand).Namespace);
        //    var manifest = cregistrar.CommitRegistrations();

        //    // assert
        //    Assert.AreEqual(cregistrar, cregistrar2);

        //    var ctypes = manifest.Commands.ToArray();
        //    Assert.AreEqual(1, ctypes.Length);
        //    Assert.AreEqual(typeof(LocalTypes.LocalCommand), ctypes[0]);

        //    var htypes = manifest.CommandHandlers.ToArray();
        //    Assert.AreEqual(1, htypes.Length);
        //    Assert.AreEqual(typeof(ICommandHandler<LocalTypes.LocalCommand>), htypes[0]);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithRecursion_ShouldRegisterInnerNamespaceHandlers()
        //{
        //    // setup
        //    var contract = new SimpleInjectorRegistrar();
        //    var sregistrar = new ServiceRegistrar(contract);
        //    var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    // test
        //    var cregistrar2 = cregistrar.AddNamespaceHandlerRegistrations(
        //        assembly: AssemblyUtil.GetTestCQRsAssembly(),
        //        @namespace: typeof(Command1Handler).Namespace,
        //        recursiveSearch: true);
        //    var manifest = cregistrar.CommitRegistrations();

        //    // assert
        //    Assert.AreEqual(cregistrar, cregistrar2);

        //    var qinfos = manifest.Commands.ToArray();
        //    Assert.AreEqual(2, qinfos.Length);

        //    var htypes = manifest.CommandHandlers.ToArray();
        //    Assert.AreEqual(2, htypes.Length);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithNullOrEmptyNamespace_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentException>(() => cregistrar.AddNamespaceHandlerRegistrations(null));
        //    //Assert.ThrowsException<ArgumentException>(() => cregistrar.AddNamespaceHandlerRegistrations(""));
        //    //Assert.ThrowsException<ArgumentException>(() => cregistrar.AddNamespaceHandlerRegistrations("  "));
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithInvalidNamespace_ShouldRegisterNoHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var cregistrar2 = cregistrar.AddNamespaceHandlerRegistrations("namespace.that.doesnt.exist");
        //    //var manifest = cregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(cregistrar, cregistrar2);

        //    //var qinfos = manifest.Commands.ToArray();
        //    //Assert.AreEqual(0, qinfos.Length);

        //    //var htypes = manifest.CommandHandlers.ToArray();
        //    //Assert.AreEqual(0, htypes.Length);
        //}
        //#endregion

        //#region AddNamespaceHandlerRegistrations(Assembly, string, RegistryScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithValidArgs_ShouldRegisterAssemblyHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var cregistrar2 = cregistrar.AddNamespaceHandlerRegistrations(
        //    //    AssemblyUtil.GetTestAssembly(),
        //    //    typeof(Command1).Namespace);
        //    //var manifest = cregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(cregistrar, cregistrar2);

        //    //var qinfos = manifest.Commands.ToArray();
        //    //Assert.AreEqual(4, qinfos.Length);

        //    //var htypes = manifest.CommandHandlers.ToArray();
        //    //Assert.AreEqual(4, htypes.Length);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithRecursion_ShouldRegisterAssemblyHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var cregistrar2 = cregistrar.AddNamespaceHandlerRegistrations(
        //    //    AssemblyUtil.GetTestAssembly(),
        //    //    typeof(Command1).Namespace,
        //    //    true);
        //    //var manifest = cregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(cregistrar, cregistrar2);

        //    //var qinfos = manifest.Commands.ToArray();
        //    //Assert.AreEqual(5, qinfos.Length);

        //    //var htypes = manifest.CommandHandlers.ToArray();
        //    //Assert.AreEqual(5, htypes.Length);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithNullAssembly_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentNullException>(() =>
        //    //    cregistrar.AddNamespaceHandlerRegistrations(
        //    //        null,
        //    //        "something"));
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithNullOrEmptyNamespace_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentException>(() =>
        //    //    cregistrar.AddNamespaceHandlerRegistrations(
        //    //        AssemblyUtil.GetTestAssembly(),
        //    //        null));
        //    //Assert.ThrowsException<ArgumentException>(() =>
        //    //    cregistrar.AddNamespaceHandlerRegistrations(
        //    //        AssemblyUtil.GetTestAssembly(),
        //    //        ""));
        //    //Assert.ThrowsException<ArgumentException>(() =>
        //    //    cregistrar.AddNamespaceHandlerRegistrations(
        //    //        AssemblyUtil.GetTestAssembly(),
        //    //        "  "));
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithInvalidNamespace_ShouldRegisterNoHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var cregistrar2 = cregistrar.AddNamespaceHandlerRegistrations(
        //    //    AssemblyUtil.GetTestAssembly(),
        //    //    "namespace.that.doesnt.exist");
        //    //var manifest = cregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(cregistrar, cregistrar2);

        //    //var qinfos = manifest.Commands.ToArray();
        //    //Assert.AreEqual(0, qinfos.Length);

        //    //var htypes = manifest.CommandHandlers.ToArray();
        //    //Assert.AreEqual(0, htypes.Length);
        //}
        //#endregion

        //#region AddAssemblyHandlerRegistrations(Assembly, RegistryScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddAssemblyHandlerRegistrations_WithValidArgs_ShouldRegisterAssemblyHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var cregistrar2 = cregistrar.AddAssemblyHandlerRegistrations(AssemblyUtil.GetTestAssembly());
        //    //var manifest = cregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(cregistrar, cregistrar2);

        //    //var qinfos = manifest.Commands.ToArray();
        //    //Assert.AreEqual(5, qinfos.Length);

        //    //var htypes = manifest.CommandHandlers.ToArray();
        //    //Assert.AreEqual(5, htypes.Length);
        //}

        //[TestMethod]
        //public void AddAssemblyHandlerRegistrations_WithNullAssembly_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentNullException>(() =>
        //    //    cregistrar.AddAssemblyHandlerRegistrations(null));
        //}
        //#endregion
    }
}
