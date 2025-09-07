using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_shop.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public DateTime dob { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string gender { get; set; }
        public string role { get; set; }
        public DateTime createAt { get; set; }

    }

    public class CreateUser
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public DateTime dob { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string gender { get; set; }
        public string role { get; set; }
    }

    public class UpdateUser
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public DateTime dob { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string gender { get; set; }
        public string role { get; set; }
    }

    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class GoogleLoginRequest
    {
        public string Credential { get; set; }
    }
}
