using System.ComponentModel.DataAnnotations;

namespace Secure_Auth_Service.DTO
{
    public class Register
    {
        [Required]
       public  string name {  get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty;
        [Required]
        public string role { get; set; } = string.Empty;
    }
}
