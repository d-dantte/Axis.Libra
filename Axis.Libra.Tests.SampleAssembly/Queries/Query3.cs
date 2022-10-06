using Axis.Libra.Query;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.SampleAssembly.Queries
{
    public class Query3: AbstractQuery
    {
        private DateTimeOffset TimeStamp { get; } = DateTimeOffset.Now;

        public string UserId { get; set; }

        /// <summary>
        /// Includes the timestamp. Note that this is only for demonstration purposes, as making the timestamp public will automatically
        /// include it in the list of serialized properties
        /// </summary>
        protected override byte[] Serialize()
        {
            return base
                .Serialize()
                .Concat(BitConverter.GetBytes(TimeStamp.UtcTicks))
                .SelectMany()
                .ToArray();
        }
    }

    public class Query3Result : IQueryResult
    {
        public string QueryURI { get; set; }

        public decimal CurrentAssetValuation { get; set; }
    }

    public class Query3Handler : IQueryHandler<Query3, Query3Result>
    {
        public Operation<Query3Result> ExecuteQuery(Query3 query) => Operation.Try(() =>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new Query3Result
            {
                QueryURI = query.QueryURI,
                CurrentAssetValuation = new SecureRandom().Using(sr => sr.NextLong())
            };
        });
    }
}
