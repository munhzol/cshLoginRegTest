using System.ComponentModel.DataAnnotations;

namespace LoginRegTest.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [Display(Name="Email")]
        public string LoginEmail {get; set;}

        [Required]
        [Display(Name="Password")]
        [DataType(DataType.Password)]
        public string LoginPassword {get; set;}

    }
}