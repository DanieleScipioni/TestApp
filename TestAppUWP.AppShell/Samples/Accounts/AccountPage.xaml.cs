using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.UserDataAccounts;

namespace TestAppUWP.AppShell.Samples.Accounts
{
    public sealed partial class AccountPage
    {
        private readonly List<string> _appointmentStoreAccessType = Enum.GetNames(typeof(UserDataAccountStoreAccessType)).ToList();

        private readonly AccountPageViewModel _accountPageViewModel;

        public AccountPage()
        {
            InitializeComponent();
            _accountPageViewModel = (AccountPageViewModel) DataContext;
        }
    }
}
