using TestAppUWP.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TestAppUWP.Core;

namespace TestAppUWP.ViewModels.Frame
{
    public class FirstPageViewModel : BindableBase
    {
        private string _add1;
        private string _add2;

        public string Add1
        {
            get { return _add1; }
            private set
            {
                if (SetProperty(ref _add1, value))
                {
                    OnPropertyChangedByName(nameof(Result));
                }
            }
        }

        public string Add2
        {
            get { return _add2; }
            private set
            {
                if (SetProperty(ref _add2, value))
                {
                    OnPropertyChangedByName(nameof(Result));
                }
            }
        }

        public string Result => _data.Result.ToString();

        public DelegateCommand Increase1Command { get; }
        public DelegateCommand Increase2Command { get; }
        public DelegateCommand Decrease1Command { get; }
        public DelegateCommand Decrease2Command { get; }

        private readonly SumClass _data;

        public FirstPageViewModel()
        {
            _data = new SumClass {Add1 = 0, Add2 = 0};
            _add1 = _data.Add1.ToString();
            _add2 = _data.Add2.ToString();

            Increase1Command = new DelegateCommand(Increase1);
            Increase2Command = new DelegateCommand(Increase2);
            Decrease1Command = new DelegateCommand(Decrease1);
            Decrease2Command = new DelegateCommand(Decrease2);
        }

        public void Increase1Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Increase1(button.CommandParameter);
        }

        public void Increase2Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Increase2(button.CommandParameter);
        }

        public void Decrease1Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Decrease1(button.CommandParameter);
        }

        public void Decrease2Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Decrease2(button.CommandParameter);
        }

        private void Increase1(object obj)
        {
            _data.Add1 += 1;
            Add1 = _data.Add1.ToString();
        }

        private void Increase2(object obj)
        {
            _data.Add2 += 1;
            Add2 = _data.Add2.ToString();
        }

        private void Decrease1(object obj)
        {
            _data.Add1 -= 1;
            Add1 = _data.Add1.ToString();
        }

        private void Decrease2(object obj)
        {
            _data.Add2 -= 1;
            Add2 = _data.Add2.ToString();
        }
    }
}