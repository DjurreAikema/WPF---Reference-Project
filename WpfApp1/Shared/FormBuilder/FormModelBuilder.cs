using System.Reflection;
using WpfApp1.Shared.FormBuilder.Validators;

namespace WpfApp1.Shared.FormBuilder;

public class FormModelBuilder
{
    /// <summary>
    /// Creates a FormGroup from a model instance, with automatic property mapping
    /// </summary>
    public static FormGroup FromModel<T>(T model) where T : class
    {
        var controls = new Dictionary<string, object>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Skip properties that cannot be read or written
            if (!property.CanRead || !property.CanWrite)
                continue;

            // Skip complex types unless they are supported primitive types
            if (!IsSupportedType(property.PropertyType))
                continue;

            try
            {
                // Get the property value
                var value = property.GetValue(model);

                // Create a form control with the property value and no validators initially
                var formControlType = typeof(FormField<>).MakeGenericType(property.PropertyType);
                var formControl = Activator.CreateInstance(formControlType, new object[] {value, null});

                // Create validators from attributes
                var validators = CreateValidatorsFromAttributes(property);

                // Add validators one by one if needed
                if (validators.Count > 0)
                {
                    var addValidatorMethod = formControlType.GetMethod("AddValidator");
                    foreach (var validator in validators)
                    {
                        try
                        {
                            addValidatorMethod.Invoke(formControl, new object[] {validator});
                        }
                        catch
                        {
                            // Ignore invalid validators
                        }
                    }
                }

                // Add the control to the dictionary
                controls.Add(property.Name.ToLowerInvariant(), formControl);
            }
            catch (Exception ex)
            {
                // Skip properties that cause exceptions
                Console.WriteLine($"Skipping property {property.Name} due to error: {ex.Message}");
            }
        }

        return new FormGroup(controls);
    }

    /// <summary>
    /// Maps values from a FormGroup back to a model instance
    /// </summary>
    public static void MapToModel<T>(FormGroup form, T model) where T : class
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Skip properties that cannot be written
            if (!property.CanWrite)
                continue;

            // Skip complex types
            if (!IsSupportedType(property.PropertyType))
                continue;

            var formFieldName = property.Name.ToLowerInvariant();

            // Check if the form has this field
            if (!form.HasControl(formFieldName))
                continue;

            // Get the value from the form field
            var formValue = form.GetValueDynamic(formFieldName);

            // If formValue is of a compatible type, set the property value
            if (formValue != null && property.PropertyType.IsAssignableFrom(formValue.GetType()))
            {
                property.SetValue(model, formValue);
            }
            else if (formValue != null)
            {
                // Try to convert the value to the property type
                try
                {
                    var convertedValue = Convert.ChangeType(formValue, property.PropertyType);
                    property.SetValue(model, convertedValue);
                }
                catch (InvalidCastException)
                {
                    // Log or handle the conversion error
                    Console.WriteLine($"Cannot convert form value to property type for {property.Name}");
                }
            }
        }
    }

    /// <summary>
    /// Creates a new instance of the model with values from the form
    /// </summary>
    public static T CreateFromForm<T>(FormGroup form) where T : class, new()
    {
        var model = new T();
        MapToModel(form, model);
        return model;
    }

    /// <summary>
    /// Updates a form with values from a model
    /// </summary>
    public static void UpdateFormFromModel<T>(FormGroup form, T model) where T : class
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Skip properties that cannot be read
            if (!property.CanRead)
                continue;

            // Skip complex types
            if (!IsSupportedType(property.PropertyType))
                continue;

            var formFieldName = property.Name.ToLowerInvariant();

            // Check if the form has this field
            if (!form.HasControl(formFieldName))
                continue;

            // Get the value from the model
            var value = property.GetValue(model);

            // Set the value in the form field
            form.SetValueDynamic(formFieldName, value);
        }
    }

    private static bool IsSupportedType(Type type)
    {
        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type);
        }

        return type.IsPrimitive ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               type.IsEnum;
    }

    private static List<object> CreateValidatorsFromAttributes(PropertyInfo property)
    {
        var validators = new List<object>();
        var validationAttributes = property.GetCustomAttributes<ValidationAttribute>(true);

        foreach (var attribute in validationAttributes)
        {
            // Create a matching validator based on the attribute type
            if (attribute is RequiredAttribute)
            {
                // For string properties
                if (property.PropertyType == typeof(string))
                {
                    var validatorType = typeof(RequiredValidator);
                    var validator = Activator.CreateInstance(validatorType, attribute.ErrorMessage);
                    validators.Add(validator);
                }
            }
            else if (attribute is MinValueAttribute minValueAttr)
            {
                // Get the min value from the attribute using reflection
                var minValueField = minValueAttr.GetType().GetField("_minValue", BindingFlags.NonPublic | BindingFlags.Instance);
                var minValue = minValueField.GetValue(minValueAttr);

                // Create a validator with the appropriate generic type
                var validatorType = typeof(MinValueValidator<>).MakeGenericType(property.PropertyType);
                var validator = Activator.CreateInstance(validatorType, minValue, attribute.ErrorMessage);
                validators.Add(validator);
            }
            else if (attribute is MaxValueAttribute maxValueAttr)
            {
                // Get the max value from the attribute using reflection
                var maxValueField = maxValueAttr.GetType().GetField("_maxValue", BindingFlags.NonPublic | BindingFlags.Instance);
                var maxValue = maxValueField.GetValue(maxValueAttr);

                // Create a validator with the appropriate generic type
                var validatorType = typeof(MaxValueValidator<>).MakeGenericType(property.PropertyType);
                var validator = Activator.CreateInstance(validatorType, maxValue, attribute.ErrorMessage);
                validators.Add(validator);
            }
            else if (attribute is StringLengthAttribute stringLengthAttr)
            {
                // Only applicable to string properties
                if (property.PropertyType == typeof(string))
                {
                    // Create a custom validator that checks string length
                    var minLengthField = stringLengthAttr.GetType().GetField("_minLength", BindingFlags.NonPublic | BindingFlags.Instance);
                    var maxLengthField = stringLengthAttr.GetType().GetField("_maxLength", BindingFlags.NonPublic | BindingFlags.Instance);

                    var minLength = (int) minLengthField.GetValue(stringLengthAttr);
                    var maxLength = (int) maxLengthField.GetValue(stringLengthAttr);

                    // Create a custom validator for string length
                    var validatorType = typeof(CustomValidator<string>);
                    var validator = Activator.CreateInstance(validatorType,
                        new Func<string, bool>(s => s == null || (s.Length >= minLength && s.Length <= maxLength)),
                        attribute.ErrorMessage);
                    validators.Add(validator);
                }
            }
            else if (attribute is EmailAttribute)
            {
                // Only applicable to string properties
                if (property.PropertyType == typeof(string))
                {
                    // Create a custom validator that checks email format
                    var validatorType = typeof(CustomValidator<string>);
                    var emailRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                    var validator = Activator.CreateInstance(validatorType,
                        new Func<string, bool>(s => s == null || emailRegex.IsMatch(s)),
                        attribute.ErrorMessage);
                    validators.Add(validator);
                }
            }
            else if (attribute is RegexAttribute regexAttr)
            {
                // Only applicable to string properties
                if (property.PropertyType == typeof(string))
                {
                    // Get the regex from the attribute using reflection
                    var regexField = regexAttr.GetType().GetField("_regex", BindingFlags.NonPublic | BindingFlags.Instance);
                    var regex = (System.Text.RegularExpressions.Regex) regexField.GetValue(regexAttr);

                    // Create a custom validator for regex
                    var validatorType = typeof(CustomValidator<string>);
                    var validator = Activator.CreateInstance(validatorType,
                        new Func<string, bool>(s => s == null || regex.IsMatch(s)),
                        attribute.ErrorMessage);
                    validators.Add(validator);
                }
            }
            // Add more validators based on other attribute types as needed
        }

        return validators;
    }
}