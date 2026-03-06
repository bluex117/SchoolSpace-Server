using backend.app.errors.http;

namespace backend.app.errors.app
{
    public class InvalidCredentialsException : UnauthorizedException
    {
        private const string DefaultMessage = "Invalid email or password.";

        public InvalidCredentialsException()
            : base(DefaultMessage) { }

        public InvalidCredentialsException(string message)
            : base(message) { }

        public InvalidCredentialsException(string message, string details)
            : base(message, details) { }
    }
}
