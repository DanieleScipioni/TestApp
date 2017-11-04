namespace TestAppUWP.Samples.Map
{
    public sealed partial class CustomerUserControl
    {
        private Customer _customer;

        public CustomerUserControl()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                var argsNewValue = args.NewValue as Customer;
                if (_customer == argsNewValue) return;
                _customer = argsNewValue;
                Bindings.Update();
            };
        }
    }
}
