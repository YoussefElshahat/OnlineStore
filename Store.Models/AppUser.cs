using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace Store.Models
{
    public class AppUser : IdentityUser
    {
        public  string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
