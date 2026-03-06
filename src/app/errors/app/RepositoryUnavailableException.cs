using backend.app.errors.http;

namespace backend.app.errors.app
{
    public class RepositoryUnavailableException : NotAvaliableException
    {
        private const string DefaultMessage = "The database is temporarily unavailable.";

        public RepositoryUnavailableException()
            : base(DefaultMessage) { }

        public RepositoryUnavailableException(string message)
            : base(message) { }

        public RepositoryUnavailableException(string message, string details)
            : base(message, details) { }
    }
}
