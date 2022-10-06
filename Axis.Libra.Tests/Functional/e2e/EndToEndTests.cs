using Axis.Libra.Command;
using Axis.Libra.Query;
using Axis.Proteus.IoC;
using Axis.Proteus.SimpleInjector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Functional.e2e
{
    [TestClass]
    public class EndToEndTests
    {
        [TestMethod]
        public async Task Test()
        {
            // setup
            //var contract = new SimpleInjectorRegistrar();
            //var sregistrar = new ServiceRegistrar(contract);

            //var cregistrar = CommandRegistrar.BeginRegistration(sregistrar);
            //var qregistrar = QueryRegistrar.BeginRegistration(sregistrar);

            //var assembly = AssemblyUtil.GetSampleAssembly();

            //cregistrar.AddAssemblyHandlerRegistrations(assembly);
            //qregistrar.AddAssemblyHandlerRegistrations(assembly);

            //_ = cregistrar.CommitRegistrations();
            //_ = qregistrar.CommitRegistrations();

            //var sresolver = sregistrar.BuildResolver();

            //var cdispatcher = new CommandDispatcher(sresolver);
            //var qdispatcher = new QueryDispatcher(sresolver);


            //#region Command1
            //var command = new SampleAssembly.Commands.Command1
            //{
            //    UserId = Guid.NewGuid().ToString(),
            //    NewAddress = "abc street, 34, Sincity",
            //    NewPhone = "987 123 456"
            //};
            //var csig = await cdispatcher.Dispatch(command);
            //Assert.AreEqual(command.CommandURI, csig);

            //var crquery = new CommandStatusQuery<SampleAssembly.Commands.Command1>(csig);
            //var cstatus = await qdispatcher.Dispatch<CommandStatusQuery<SampleAssembly.Commands.Command1>, CommandStatusResult>(crquery);
            //Assert.AreEqual(ICommandStatus.Succeeded, cstatus.Status);
            //Assert.AreEqual(crquery.QueryURI, cstatus.QueryURI);
            //Assert.AreEqual(crquery.CommandURI, cstatus.CommandSignature);
            //#endregion

            //#region Query2
            //var query2 = new SampleAssembly.Queries.Query2
            //{
            //    SearchDuration = TimeSpan.FromDays(4.2),
            //    PageIndex = 2,
            //    PageSize = 20,
            //    SearchStart = DateTimeOffset.Now - TimeSpan.FromDays(12),
            //    SearchString = "Betaal"
            //};
            //var q2Result = await qdispatcher.Dispatch<SampleAssembly.Queries.Query2, SampleAssembly.Queries.Query2Result>(query2);
            //Assert.IsNotNull(q2Result);
            //Assert.AreEqual(query2.QueryURI, q2Result.QueryURI);
            //#endregion
        }

    }
}
