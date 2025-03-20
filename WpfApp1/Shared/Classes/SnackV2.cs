using System.ComponentModel.DataAnnotations;
using WpfApp1.Shared.FormBuilder.Validators;

namespace WpfApp1.Shared.Classes
{
    public class SnackV2
    {
        // --- Internal
        [Key] public int? Id { get; set; }

        // --- Form
        [FormBuilder.Validators.Required(ErrorMessage = "Name is required")]
        [FormBuilder.Validators.StringLength(50, minLength: 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [FormBuilder.Validators.Required(ErrorMessage = "Price is required")]
        [MinValue(0.01, ErrorMessage = "Price must be greater than zero")]
        public double Price { get; set; }

        [FormBuilder.Validators.Required(ErrorMessage = "Quantity is required")]
        [MinValue(0, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        public SnackV2()
        {
        }

        public SnackV2(SnackV2 other)
        {
            Id = other.Id;
            Name = other.Name;
            Price = other.Price;
            Quantity = other.Quantity;
        }
    }
}