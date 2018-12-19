using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace SaveWise.DataLayer.Models.Users
{
    public class User : Document
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [BsonIgnore]
        public string Password { get; set; }
        
        public byte[] PasswordHash { get; set; }
        
        public byte[] PasswordSalt { get; set; }
    }
}