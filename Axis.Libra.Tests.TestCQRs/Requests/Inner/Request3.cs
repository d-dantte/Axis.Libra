namespace Axis.Libra.Tests.TestCQRs.Requests.Inner
{
    public class Request3
    {
        public Guid Id { get; set; }
    }

    public class Request3Handler
    {
        public Request3Result ExecuteRequest(Request3 request)
        {
            Console.WriteLine($"{typeof(Request1)} handler executed.");
            return new Request3Result
            {
                Meh = "bleh"
            };
        }

        public Request2Result ExecuteRequest(Request2 request)
        {
            Console.WriteLine($"{typeof(Request1)} handler executed.");
            return new Request2Result
            {
                TimeStamp = DateTimeOffset.Now
            };
        }
    }

    public class Request3Result
    {
        public string? Meh { get; set; }
    }
}
