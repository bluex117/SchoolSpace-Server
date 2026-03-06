using backend.app.errors.http;

namespace backend.app.errors.app
{
    public class SelfReportException : BadRequestException
    {
        private const string DefaultMessage = "You cannot report yourself.";

        public SelfReportException()
            : base(DefaultMessage) { }

        public SelfReportException(string message)
            : base(message) { }

        public SelfReportException(string message, string details)
            : base(message, details) { }
    }
}
