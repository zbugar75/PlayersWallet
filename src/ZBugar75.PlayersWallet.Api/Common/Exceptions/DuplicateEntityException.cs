using System;
using System.Runtime.Serialization;

namespace Zbugar75.PlayersWallet.Api.Common.Exceptions
{
    [Serializable]
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException() { }

        public DuplicateEntityException(string message)
            : base(message) { }

        public DuplicateEntityException(string message, Exception inner)
            : base(message, inner) { }

        protected DuplicateEntityException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
