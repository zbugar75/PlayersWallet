using System;
using System.Runtime.Serialization;

namespace Zbugar75.PlayersWallet.Api.Exceptions
{
    [Serializable]
    public class DuplicateUsernameException : Exception
    {
        public DuplicateUsernameException() { }

        public DuplicateUsernameException(string message)
            : base(message) { }

        public DuplicateUsernameException(string message, Exception inner)
            : base(message, inner) { }

        protected DuplicateUsernameException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
