namespace WebAppApiArq.Models
{

    public class AuthResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }

    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public RefreshTokenDTO RefreshToken { get; set; }
    }

    public class RefreshTokenDTO
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
