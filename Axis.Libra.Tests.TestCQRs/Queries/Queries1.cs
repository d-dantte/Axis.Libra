using Axis.Libra.Instruction;
using HashDepot;
using Axis.Luna.Extensions;

namespace Axis.Libra.Tests.TestCQRs.Queries
{
    public class Query1
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

    public class Query1Handler
    {
        public async Task<Query1Result> ExecuteQuery(Query1 query)
        {
            Console.WriteLine($"{typeof(Query1)} handler executed.");
            await Task.Delay(1);
            return new Query1Result
            {
                Stuff = 34
            };
        }
    }
}
