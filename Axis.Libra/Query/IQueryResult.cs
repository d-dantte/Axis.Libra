namespace Axis.Libra.Query
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IQueryResult<TResult>
    {
        string QuerySignature { get; }
        TResult Result { get; }
    }
}
