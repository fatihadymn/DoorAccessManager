namespace DoorAccessManager.Items.Authentication
{
    public class JwtOptions
    {
        public string SecretKey { get; set; }

        public string ValidIssuer { get; set; }

        public int ExpirySeconds { get; set; }
    }
}
