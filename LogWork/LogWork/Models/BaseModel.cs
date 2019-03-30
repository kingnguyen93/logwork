using SQLite;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVVM;

namespace LogWork.Models
{
    public class BaseModel : TinyModel
    {
        [Ignore]
        public virtual string[] ExcludedProperties { get; set; }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", params string[] relatedProperty)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);

            foreach (var property in relatedProperty)
            {
                OnPropertyChanged(property);
            }

            return true;
        }
    }
}