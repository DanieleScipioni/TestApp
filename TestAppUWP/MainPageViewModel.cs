using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TestAppUWP
{
    internal sealed class MainPageViewModel : ICommand, INotifyPropertyChanged
    {
        public ICommand OpenDialog => this;


        public string LockScreenLogo = "ms-appx:///Assets/LockScreenLogo.scale-200.png";

        public string X = "x";

        public string Null = null;

        public string Text { get; } = "cavoletti ";

        public ObservableCollection<string> List = new ObservableCollection<string>()
        {
            "a", "b", "c"
        };

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //var stringParameter = parameter as string;
            //if (stringParameter == null) return;

            //int intParemter;
            //if (!int.TryParse(stringParameter, out intParemter)) return;

            //switch (intParemter)
            //{
            //    case 0:
            //        var contentDialog = new ContentDialog
            //        {
            //            Style = Application.Current.Resources["ContentDialogStyle"] as Style
            //        };
            //        await contentDialog.ShowAsync();
            //        break;
            //}
        }

        public event EventHandler CanExecuteChanged;

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChangedByName(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChangedByName(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
