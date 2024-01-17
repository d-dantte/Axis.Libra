namespace Axis.Libra.Tests.TestCQRs.Queries
{
    public class Query2
    {
        public Uri? Something { get; set; }
    }

    public class Query2Result
    {
        public DateTimeOffset TimeStamp { get; set; }
    }

    public class Query2Handler
    {
        public async Task<Query2Result> ExecuteQuery(Query2 query)
        {
            Console.WriteLine($"{typeof(Query2)} handler executed.");
            await Task.Delay(1);
            return new Query2Result
            {
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}
