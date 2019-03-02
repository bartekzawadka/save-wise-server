using System;

namespace SaveWise.DataLayer.Models.Users
{
    public class ApiToken
    {
        public string Token { get; set; }
        
        public DateTime? Expires { get; set; }
        
        public string Username { get; set; }
        
        public string Id { get; set; }
    }
}