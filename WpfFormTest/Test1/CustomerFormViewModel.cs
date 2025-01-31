using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace WpfFormTest.Test1
{
    public class CustomerFormViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private Customer _customer;
        private Dictionary<string, List<string>> _errors;

        public CustomerFormViewModel()
        {
            _customer = new Customer();
            _errors = new Dictionary<string, List<string>>();

            SubmitCommand = new RelayCommand(Submit, CanSubmit);
        }

        #region Public Properties (Bound to View)

        public string FirstName
        {
            get => _customer.FirstName;
            set
            {
                if (_customer.FirstName != value)
                {
                    _customer.FirstName = value;
                    OnPropertyChanged(nameof(FirstName));
                    Validate();
                }
            }
        }

        public string LastName
        {
            get => _customer.LastName;
            set
            {
                if (_customer.LastName != value)
                {
                    _customer.LastName = value;
                    OnPropertyChanged(nameof(LastName));
                    Validate();
                }
            }
        }

        public string Email
        {
            get => _customer.Email;
            set
            {
                if (_customer.Email != value)
                {
                    _customer.Email = value;
                    OnPropertyChanged(nameof(Email));
                    Validate();
                }
            }
        }

        #endregion

        #region Validation (INotifyDataErrorInfo)

        public bool HasErrors => _errors.Any(kv => kv.Value != null && kv.Value.Count > 0);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && _errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }
            else if (string.IsNullOrEmpty(propertyName))
            {
                // Return all errors if no property name is specified
                return _errors.SelectMany(err => err.Value).ToList();
            }
            return null;
        }

        private void Validate()
        {
            // Clear existing errors
            var propertyNames = _errors.Keys.ToList();
            _errors.Clear();

            // Validate the entire Customer object
            var validationContext = new ValidationContext(_customer, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(_customer, validationContext, validationResults, true);

            // Convert validation results to dictionary entries
            foreach (var result in validationResults)
            {
                foreach (var memberName in result.MemberNames)
                {
                    if (!_errors.ContainsKey(memberName))
                    {
                        _errors[memberName] = new List<string>();
                    }
                    _errors[memberName].Add(result.ErrorMessage);
                }
            }

            // Raise ErrorsChanged for all properties that might have changed
            var allProperties = new HashSet<string>(propertyNames.Concat(_errors.Keys));
            foreach (var propertyName in allProperties)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            // Also raise CanExecuteChanged for the SubmitCommand
            (SubmitCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        #endregion

        #region Commands

        public RelayCommand SubmitCommand { get; }

        private bool CanSubmit(object parameter)
        {
            // For example, disable the button if any errors exist
            return !HasErrors;
        }

        private void Submit(object parameter)
        {
            // Example action: show a MessageBox on submit
            // In a real app, you'd send _customer to a service, etc.
            MessageBox.Show(
                $"Submitted!\nName: {_customer.FirstName} {_customer.LastName}\nEmail: {_customer.Email}",
                "Form Submission",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
