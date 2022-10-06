using Axis.Libra.Query;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using System;

namespace Axis.Libra.Tests.SampleAssembly.Queries
{
    public class Query1: AbstractQuery 
    { 
        public string Id { get; set; }
    }

    public class Query1Result : IQueryResult
    {
        public string QueryURI { get; set; }

        public string UserProfile { get; set; }
    }

    public class Query1Handler : IQueryHandler<Query1, Query1Result>
    {
        public Operation<Query1Result> ExecuteQuery(Query1 query) => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new Query1Result
            {
                QueryURI = query.QueryURI,
                UserProfile = $"@{new SecureRandom().Using(sr => sr.NextAlphaNumericString(12))}"
            };
        });
    }
}
