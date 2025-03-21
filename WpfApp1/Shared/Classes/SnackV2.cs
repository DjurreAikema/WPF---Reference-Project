using System.ComponentModel.DataAnnotations;
using WpfApp1.Shared.FormBuilder;
using WpfApp1.Shared.FormBuilder.Validators;

namespace WpfApp1.Shared.Classes
{
    public class SnackV2
    {
        // --- Internal id (excluded from form)
        [FormExclude]
        [Key]
        public int? Id { get; set; }

        // --- Form fields with custom labels and descriptions
        [FormProperty("Snack Name", 1, "Enter the name of the snack product")]
        [FormBuilder.Validators.Required(ErrorMessage = "Name is required")]
        [FormBuilder.Validators.StringLength(50, minLength: 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [FormProperty("Price ($)", 2, "Enter the retail price in dollars")]
        [FormBuilder.Validators.Required(ErrorMessage = "Price is required")]
        [MinValue(0.01, ErrorMessage = "Price must be greater than zero")]
        public double Price { get; set; }

        [FormProperty("Quantity in Stock", 3, "Number of items currently in inventory")]
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