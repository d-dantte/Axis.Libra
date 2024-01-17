namespace Axis.Libra.Tests.TestCQRs.Queries.Inner
{
    public class Query3
    {
        public Guid Id { get; set; }
    }

    public class Query3Handler
    {
        public async Task<Query3Result> ExecuteQuery(Query3 query)
        {
            Console.WriteLine($"{typeof(Query1)} handler executed.");
            await Task.Delay(1);
            return new Query3Result
            {
                Meh = "bleh"
            };
        }

        public async Task<Query2Result> ExecuteQuery(Query2 query)
        {
            Console.WriteLine($"{typeof(Query1)} handler executed.");
            await Task.Delay(1);
            return new Query2Result
            {
                TimeStamp = DateTimeOffset.Now
            };
        }
    }

    public class Query3Result
    {
        public string? Meh { get; set; }
    }
}
