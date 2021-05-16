using System;
using System.Runtime.Serialization;

namespace Zbugar75.PlayersWallet.Api.Common.Exceptions
{
    [Serializable]
    public class LowBalanceException : Exception
    {
        public LowBalanceException() { }

        public LowBalanceException(string message)
            : base(message) { }

        public LowBalanceException(string message, Exception inner)
            : base(message, inner) { }

        protected LowBalanceException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
