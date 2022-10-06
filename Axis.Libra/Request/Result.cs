using System;
using System.Collections.Generic;

namespace Axis.Libra.Request
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResult
    {
        public static IResult OfSuccess() => default(Succeeded);

        public static IResult Of<T>(T data) => new DataResult<T>(data);

        public static IResult Of(RequestException exception) => new ErrorResult(exception);


        #region Union Types

        /// <summary>
        /// Result that represents a successful operation that doesn't have any data to return.
        /// </summary>
        public struct Succeeded : IResult
        {
            public override string ToString() => "Succeeded";

            #region record struct
            public override int GetHashCode() => 0;

            public override bool Equals(object obj) => obj is Succeeded;

            public static bool operator ==(Succeeded a, Succeeded b) => a.Equals(b);

            public static bool operator !=(Succeeded a, Succeeded b) => !(a == b);
            #endregion
        }

        /// <summary>
        /// Result that represents data.
        /// </summary>
        /// <typeparam name="TData">The type of the returned data</typeparam>
        public struct DataResult<TData>: IResult
        {
            /// <summary>
            /// The data
            /// </summary>
            public TData Data { get; }

            public DataResult(TData data)
            {
                Data = data;
            }

            public override string ToString() => Data?.ToString();

            #region record struct
            public override int GetHashCode() => Data?.GetHashCode() ?? 0;

            public override bool Equals(object obj)
                => obj is DataResult<TData> dataResult
                && EqualityComparer<TData>.Default.Equals(Data, dataResult.Data);

            public static bool operator ==(DataResult<TData> a, DataResult<TData> b) => a.Equals(b);

            public static bool operator !=(DataResult<TData> a, DataResult<TData> b) => !(a == b);
            #endregion
        }

        /// <summary>
        /// Restul that represents a faulted operation. The exception is encapsulated within this result.
        /// </summary>
        public struct ErrorResult: IResult
        {
            private RequestException _exception;

            /// <summary>
            /// The exception message.
            /// </summary>
            public string Message => _exception?.Message;

            /// <summary>
            /// The exception code, if any is given
            /// </summary>
            public string Code => _exception?.Code;

            /// <summary>
            /// The encapsulated exception
            /// </summary>
            /// <returns></returns>
            public RequestException Exception() => _exception;

            public ErrorResult(RequestException exception)
            {
                _exception = exception ?? throw new ArgumentNullException($"invalid '{nameof(exception)}' argument");
            }

            #region record struct
            public override int GetHashCode() => HashCode.Combine(
                Message,
                Code,
                _exception?.InnerException?.ToString());

            public override bool Equals(object obj)
                => obj is ErrorResult result
                && result.Code.IsNullOrEqual(Code)
                && result.Message.IsNullOrEqual(Message)
                && (result._exception?.InnerException?.ToString()) // the extra brackets are to explicitly exit the context of Nullable<T> created by the '?.' operator
                    .IsNullOrEqual(_exception?.InnerException?.ToString()); 

            public static bool operator ==(ErrorResult a, ErrorResult b) => a.Equals(b);

            public static bool operator !=(ErrorResult a, ErrorResult b) => !(a == b);
            #endregion
        }
        #endregion
    }
}
