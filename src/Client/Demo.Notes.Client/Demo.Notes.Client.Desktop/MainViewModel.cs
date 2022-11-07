using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Demo.Notes.Client.Desktop
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ClaimsPrincipal? _user;
        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        public ClaimsPrincipal? User
        {
            get => _user;
            set
            {
                SetField(ref _user, value);
                NotifyOfPropertyChange(nameof(IsLoggedIn));
            }
        }
        
        public bool IsLoggedIn => User != null;
        public UserData? Tokens { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyOfPropertyChange([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            NotifyOfPropertyChange(propertyName);
            return true;
        }
        #endregion
    }
}
