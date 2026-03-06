namespace backend.app.dtos.respones.auth
{
    public class AuthResponse
    {
        public AuthResponse(int id, string username, string role, string token)
        {
            Id = id;
            Username = username;
            Role = role;
            Token = token;
            Avatar = "placeholder";
        }

        public int Id
        {
            get; set;
        }
        public string Username
        {
            get; set;
        }
        public string Role
        {
            get; set;
        }
        public string Token
        {
            get; set;
        }
        public string Avatar
        {
            get; set;
        }
    }
}