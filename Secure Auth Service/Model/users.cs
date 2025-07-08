namespace Secure_Auth_Service.Model
{
    public class users
    {
       public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public DateTime created_at { get; set; } = DateTime.Now;

    }
}
