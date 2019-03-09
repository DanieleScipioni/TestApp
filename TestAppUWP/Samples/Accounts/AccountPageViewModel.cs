using System;
using System.Collections.Generic;
using System.Linq;
using TestAppUWP.Core;
using Windows.ApplicationModel.UserDataAccounts;

namespace TestAppUWP.Samples.Accounts
{
    public class AccountPageViewModel : BindableBase
    {
        public AccountPageViewModel()
        {
            _selectedStoreType = UserDataAccountStoreAccessType.AllAccountsReadOnly.ToString();
            LoadUserDataAccouns();
        }

        private string _selectedStoreType;
        public string SelectedStoreType
        {
            get => _selectedStoreType;
            set
            {
                if (!SetProperty(ref _selectedStoreType, value)) return;
                LoadUserDataAccouns();
            }
        }

        public IEnumerable<UserDataAccountAdapter> UserDataAccounts { get; private set; }

        public async void LoadUserDataAccouns()
        {
            if (!Enum.TryParse(_selectedStoreType, out UserDataAccountStoreAccessType userDataAccountStoreAccessType)) return;
            var userDataAccountStore = await UserDataAccountManager.RequestStoreAsync(userDataAccountStoreAccessType);
            UserDataAccounts = (await userDataAccountStore.FindAccountsAsync()).Select(account => new UserDataAccountAdapter(account));
            OnPropertyChangedByName(nameof(UserDataAccounts));
        }
    }
}