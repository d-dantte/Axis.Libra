using Axis.Libra.Query;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using System;
using System.Linq;

namespace Axis.Libra.Tests.SampleAssembly.Queries
{
    using LinqEnumerable = System.Linq.Enumerable;
    public class Query2: AbstractQuery
    {
        public string SearchString { get; set; }
        public DateTimeOffset? SearchStart { get; set; }
        public TimeSpan? SearchDuration { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

    public class Query2Result : IQueryResult
    {
        public string QueryURI { get; set; }

        public  Page<string> ResultPage { get; set; }
    }

    public class Query2Handler : IQueryHandler<Query2, Query2Result>
    {
        public Operation<Query2Result> ExecuteQuery(Query2 query) => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new Query2Result
            {
                QueryURI = query.QueryURI,
                ResultPage = LinqEnumerable
                    .Range(0, query.PageSize)
                    .Select(i => $"Result: {i}")
                    .ApplyTo(results => new Page<string>(
                        query.PageIndex,
                        results.ToArray()))
            };
        });
    }
}
