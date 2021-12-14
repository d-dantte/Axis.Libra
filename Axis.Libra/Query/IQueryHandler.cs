using Axis.Luna.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Query
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    public interface IQueryHandler<TQuery, TResult>
    where TQuery: IQuery
    {
        Operation<IQueryResult<TResult>> ExecuteQuery(TQuery query);
    }
}
