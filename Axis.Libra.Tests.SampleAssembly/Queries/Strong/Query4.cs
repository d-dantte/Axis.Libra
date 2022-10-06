using Axis.Libra.Query;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using System;

namespace Axis.Libra.Tests.SampleAssembly.Queries.Strong
{
    public class Query4 : AbstractQuery
    {
        public string Id { get; set; }
    }

    public class Query4Result : IQueryResult
    {
        public string QueryURI { get; set; }

        public string UserProfile { get; set; }
    }

    public class Query4Handler : IQueryHandler<Query4, Query4Result>
    {
        public Operation<Query4Result> ExecuteQuery(Query4 query) => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new Query4Result
            {
                QueryURI = query.QueryURI,
                UserProfile = $"@{new SecureRandom().Using(sr => sr.NextAlphaNumericString(12))}"
            };
        });
    }
}
