using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace Calame
{
    static public class NotifyPropertyChangedExtension
    {
        static public bool SetValue<T>(this INotifyPropertyChangedEx instance, ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            field = newValue;
            instance.NotifyOfPropertyChange(propertyName);
            return true;
        }
    }
}