namespace TestAppUWP.Data
{
    public enum NavigationType
    {
        Breadcrumb,
        VerticalList,
        HorizontalList
    }

    public class BasePageParameter
    {
        private NavigationType _navigationType;
        public NavigationType ViewNavigationType
        {
            get { return _navigationType; }
            set { _navigationType = value; }
        }

        public bool IsNavigationOn { get; set; }
    }
}
