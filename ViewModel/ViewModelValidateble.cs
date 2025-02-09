using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.ViewModel
{
    internal abstract class ViewModelValidateble : ViewModelBase,
        INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public bool HasErrors => Errors.Count > 0;
        /// <summary>
        /// Событие, которое срабатывает каждый раз, когда валидация кокого-либо свойства при помощи метода Validate
        /// </summary>
        protected event Action? PostValidationChange;
        protected Dictionary<string, List<string>> Errors { get => _errors; set => _errors = value; }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                return Errors[propertyName];
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        protected void Validate(string propertyName,object value)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(value,new ValidationContext(this) { MemberName = propertyName},results);

            if (results.Any())
            {
                if (!Errors.ContainsKey(propertyName)) Errors.Add(propertyName, results.Select(x => x.ErrorMessage).ToList());
                ErrorsChanged?.Invoke(this,new DataErrorsChangedEventArgs(propertyName));
            }
            else 
            { 
                Errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            PostValidationChange?.Invoke();
        }

        /// <summary>
        /// Метод, который возвращает истинну, когда все условия валидации на модели соблюдены, рекомендуется использовать
        /// вместе с commands
        /// </summary>
        /// <returns></returns>
        protected bool CanExecuteByValidation()
        {
            return Validator.TryValidateObject(this,new ValidationContext(this),null) && !HasErrors;
        }
    }
}
