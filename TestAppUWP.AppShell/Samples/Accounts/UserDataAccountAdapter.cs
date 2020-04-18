using System;
using TestAppUWP.Core;
using Windows.ApplicationModel.UserDataAccounts;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.AppShell.Samples.Accounts
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
            private set => SetProperty(ref _accountIconVisibile, value);
        }

        private async void LoadIconFromAccount(UserDataAccount userDataAccount)
        {
            IRandomAccessStreamReference randomAccessStreamReference = userDataAccount.Icon;
            if (randomAccessStreamReference == null) return;
            var bitmapImage = new BitmapImage();
            using (IRandomAccessStreamWithContentType openReadAsync = await randomAccessStreamReference.OpenReadAsync())
            {
                bitmapImage.SetSource(openReadAsync);
            }
            _bitmapImage = bitmapImage;
            OnPropertyChangedByName(nameof(BitmapImage));
            AccountIconVisibile = true;
        }
    }
}