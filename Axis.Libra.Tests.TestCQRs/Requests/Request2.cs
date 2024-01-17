namespace Axis.Libra.Tests.TestCQRs.Requests
{
    public class Request2
    {
        public Uri? Something { get; set; }
    }

    public class Request2Result
    {
        public DateTimeOffset TimeStamp { get; set; }
    }

    public class Request2Handler
    {
        public Request2Result ExecuteRequest(Request2 request)
        {
            Console.WriteLine($"{typeof(Request2)} handler executed.");
            return new Request2Result
            {
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}
