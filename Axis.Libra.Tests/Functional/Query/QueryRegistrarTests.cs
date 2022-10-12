using Axis.Libra.Query;
using Axis.Proteus.IoC;
using Axis.Proteus.SimpleInjector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace Axis.Libra.Tests.Functional.Query
{
    [TestClass]
    public class QueryRegistrarTests
    {
        //[TestMethod]
        //public void BeginRegistration_WithValidServiceRegistrar_ShouldReturnValidQueryRegistrar()
        //{
        //    //// setup
        //    //var contract = new Mock<IRegistrarContract>();
        //    //var sregistrar = new ServiceRegistrar(contract.Object);

        //    //// test
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// assert
        //    //Assert.IsNotNull(qregistrar);
        //}

        //#region AddHandlerRegistration<TImpl, TQuery, TQueryResult>(RegistryScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddHandlerRegistration_ShouldRegisterGivenType()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddHandlerRegistration<Query1Handler, Query1, Query1Result>();
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(1, qinfos.Length);
        //    //Assert.AreEqual(typeof(Query1), qinfos[0].QueryType);
        //    //Assert.AreEqual(typeof(Query1Result), qinfos[0].QueryResultType);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(1, htypes.Length);
        //    //Assert.AreEqual(typeof(IQueryHandler<Query1, Query1Result>), htypes[0]);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}
        //#endregion

        //#region AddHandlerRegistration(Type, RegistrationScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddHandlerRegistration2_WithValidTypes_ShouldRegisterGivenType()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddHandlerRegistration(typeof(Query3Handler));
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(1, qinfos.Length);
        //    //Assert.AreEqual(typeof(Query3), qinfos[0].QueryType);
        //    //Assert.AreEqual(typeof(Query3Result), qinfos[0].QueryResultType);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(1, htypes.Length);
        //    //Assert.AreEqual(typeof(IQueryHandler<Query3, Query3Result>), htypes[0]);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}

        //[TestMethod]
        //public void AddHandlerRegistration2_WithInvalidType_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test & assert
        //    //Assert.ThrowsException<ArgumentException>(() => qregistrar.AddHandlerRegistration(typeof(Command1Handler)));
        //}
        //#endregion

        //#region AddNamespaceHandlerRegistrations(string, RegistryScope?, InterceptorProfile? bool)
        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithValidArgument_ShouldRegisterFoundHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddNamespaceHandlerRegistrations(typeof(Samples.Query1Handler).Namespace);
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(1, qinfos.Length);
        //    //Assert.AreEqual(typeof(Samples.Query1), qinfos[0].QueryType);
        //    //Assert.AreEqual(typeof(Samples.Query1Result), qinfos[0].QueryResultType);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(1, htypes.Length);
        //    //Assert.AreEqual(typeof(IQueryHandler<Samples.Query1, Samples.Query1Result>), htypes[0]);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithRecursion_ShouldRegisterInnerNamespaceHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddNamespaceHandlerRegistrations(
        //    //    @namespace: typeof(Samples.Query1Handler).Namespace,
        //    //    recursiveSearch: true);
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(2, qinfos.Length);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(2, htypes.Length);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithNullOrEmptyNamespace_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentException>(() => qregistrar.AddNamespaceHandlerRegistrations(null));
        //    //Assert.ThrowsException<ArgumentException>(() => qregistrar.AddNamespaceHandlerRegistrations(""));
        //    //Assert.ThrowsException<ArgumentException>(() => qregistrar.AddNamespaceHandlerRegistrations("  "));
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations_WithInvalidNamespace_ShouldRegisterNoHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddNamespaceHandlerRegistrations("namespace.that.doesnt.exist");
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(0, qinfos.Length);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(0, htypes.Length);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}
        //#endregion

        //#region AddNamespaceHandlerRegistrations(Assembly, string, RegistryScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithValidArgs_ShouldRegisterAssemblyHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddNamespaceHandlerRegistrations(
        //    //    AssemblyUtil.GetSampleAssembly(),
        //    //    typeof(Query1).Namespace);
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(3, qinfos.Length);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(3, htypes.Length);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithRecursion_ShouldRegisterAssemblyHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddNamespaceHandlerRegistrations(
        //    //    AssemblyUtil.GetSampleAssembly(),
        //    //    typeof(Query1).Namespace,
        //    //    true);
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(4, qinfos.Length);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(4, htypes.Length);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithNullAssembly_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentNullException>(() =>
        //    //    qregistrar.AddNamespaceHandlerRegistrations(
        //    //        null,
        //    //        "something"));
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithNullOrEmptyNamespace_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentException>(() =>
        //    //    qregistrar.AddNamespaceHandlerRegistrations(
        //    //        AssemblyUtil.GetSampleAssembly(),
        //    //        null));
        //    //Assert.ThrowsException<ArgumentException>(() =>
        //    //    qregistrar.AddNamespaceHandlerRegistrations(
        //    //        AssemblyUtil.GetSampleAssembly(),
        //    //        ""));
        //    //Assert.ThrowsException<ArgumentException>(() =>
        //    //    qregistrar.AddNamespaceHandlerRegistrations(
        //    //        AssemblyUtil.GetSampleAssembly(),
        //    //        "  "));
        //}

        //[TestMethod]
        //public void AddNamespaceHandlerRegistrations2_WithInvalidNamespace_ShouldRegisterNoHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddNamespaceHandlerRegistrations(
        //    //    AssemblyUtil.GetSampleAssembly(),
        //    //    "namespace.that.doesnt.exist");
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(0, qinfos.Length);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(0, htypes.Length);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(0, chtypes.Length);
        //}
        //#endregion

        //#region AddAssemblyHandlerRegistrations(Assembly, RegistryScope?, InterceptorProfile?)
        //[TestMethod]
        //public void AddAssemblyHandlerRegistrations_WithValidArgs_ShouldRegisterAssemblyHandlers()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //var qregistrar2 = qregistrar.AddAssemblyHandlerRegistrations(AssemblyUtil.GetSampleAssembly());
        //    //var manifest = qregistrar.CommitRegistrations();

        //    //// assert
        //    //Assert.AreEqual(qregistrar, qregistrar2);

        //    //var qinfos = manifest.Queries.ToArray();
        //    //Assert.AreEqual(9, qinfos.Length);

        //    //var htypes = manifest.QueryHandlers.ToArray();
        //    //Assert.AreEqual(9, htypes.Length);

        //    //var chtypes = manifest.CommandResultHandlers.ToArray();
        //    //Assert.AreEqual(5, chtypes.Length);
        //}

        //[TestMethod]
        //public void AddAssemblyHandlerRegistrations_WithNullAssembly_ShouldThrowException()
        //{
        //    //// setup
        //    //var contract = new SimpleInjectorRegistrar();
        //    //var sregistrar = new ServiceRegistrar(contract);
        //    //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

        //    //// test
        //    //Assert.ThrowsException<ArgumentNullException>(() =>
        //    //    qregistrar.AddAssemblyHandlerRegistrations(null));
        //}
        //#endregion
    }
}
