using System.ComponentModel.DataAnnotations;

namespace WpfFormTest.Test1
{
    public class Customer
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(3, ErrorMessage = "First name must be at least 3 characters long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(3, ErrorMessage = "Last name must be at least 3 characters long")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }
    }
}