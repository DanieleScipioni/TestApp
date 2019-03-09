using System;
using Windows.ApplicationModel.UserDataAccounts;
using Windows.UI.Xaml.Media.Imaging;
using TestAppUWP.Core;

namespace TestAppUWP.Samples.Accounts
{
    public class UserDataAccountAdapter : BindableBase
    {
        public UserDataAccountAdapter(UserDataAccount userDataAccount)
        {
            UserDataAccount = userDataAccount;
        }

        public readonly UserDataAccount UserDataAccount;

        private BitmapImage _bitmapImage;
        public BitmapImage BitmapImage
        {
            get
            {
                if (_bitmapImage != null) return _bitmapImage;
                LoadIconFromAccount(UserDataAccount);
                return null;
            }
        }

        private bool _accountIconVisibile;
        public bool AccountIconVisibile
        {
            get => _accountIconVisibile;
            set => SetProperty(ref _accountIconVisibile, value);
        }

        private async void LoadIconFromAccount(UserDataAccount userDataAccount)
        {
            var randomAccessStreamReference = userDataAccount.Icon;
            if (randomAccessStreamReference == null) return;
            var bitmapImage = new BitmapImage();
            using (var openReadAsync = await randomAccessStreamReference.OpenReadAsync())
            {
                bitmapImage.SetSource(openReadAsync);
            }
            _bitmapImage = bitmapImage;
            OnPropertyChangedByName(nameof(BitmapImage));
            AccountIconVisibile = true;
        }
    }
}