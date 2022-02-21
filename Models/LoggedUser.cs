using System;
using System.ComponentModel.DataAnnotations;

namespace login_and_registration.Models
{
    public class LoggedUser
    {
        [Required]
        [EmailAddress]
        public string LoggedEmail {get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string LoggedPassword {get;set;}
    }
}