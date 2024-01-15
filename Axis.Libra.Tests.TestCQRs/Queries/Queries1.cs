using Axis.Libra.Query;
using Axis.Libra.Instruction;
using Axis.Luna.Common.Results;
using HashDepot;
using Axis.Luna.Extensions;

namespace Axis.Libra.Tests.TestCQRs.Queries
{
    public class Query1: IQuery<Query1Result>
    {
        public int Age { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }

        public static InstructionNamespace InstructionNamespace() => "axis.libra.test-crs.query1";

        public InstructionHash InstructionHash()
        {
            return BitConverter
                .GetBytes(Age)
                .Concat(BitConverter.GetBytes(ExpiryDate.Ticks))
                .ToArray()
                .ApplyTo(bytes => XXHash.Hash64(bytes));
        }
    }

    public class Query1Result
    {
        public int Stuff { get; set; }
    }

    public class Query1Handler : IQueryHandler<Query1, Query1Result>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IResult<Query1Result>> ExecuteQuery(Query1 query)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Console.WriteLine($"{typeof(Query1)} handler executed.");
            return Result.Of(new Query1Result
            {
                Stuff = 34
            });
        }
    }
}
