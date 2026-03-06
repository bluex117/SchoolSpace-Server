using backend.app.errors.http;

namespace backend.app.errors.app
{
    public class RepositoryWriteException : NotAvaliableException
    {
        private const string DefaultMessage = "Unable to write to the database.";

        public RepositoryWriteException()
            : base(DefaultMessage) { }

        public RepositoryWriteException(string message)
            : base(message) { }

        public RepositoryWriteException(string message, string details)
            : base(message, details) { }
    }
}
