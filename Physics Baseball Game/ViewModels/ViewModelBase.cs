using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Physics_Baseball_Game.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels. Provides INotifyPropertyChanged implementation
    /// and a SetProperty helper to simplify property backing field updates.
    /// </summary>
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Sets the backing field if the value has changed and raises PropertyChanged.
        /// </summary>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value!;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises PropertyChanged for the supplied property name.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
