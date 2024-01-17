namespace Axis.Libra.Tests.TestCQRs.Requests
{
    public class Request1
    {
        public int Age { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }
    }

    public class Request1Result
    {
        public int Stuff { get; set; }
    }

    public class Request1Handler
    {
        public Request1Result ExecuteRequest(Request1 request)
        {
            Console.WriteLine($"{typeof(Request1)} handler executed.");
            return new Request1Result
            {
                Stuff = 34
            };
        }
    }
}
