namespace Secure_Auth_Service.Model
{
    public class refresh_tokens
    {
        public int id { get; set; }
        public string token { get; set; } = string.Empty;

        public DateTime expires_at { get; set;} 

        public int is_revoked { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;

        public int user_id { get; set; }


    }
}
