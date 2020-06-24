using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.AppShell.Samples.Test
{
    public partial class TabViewUi
    {
        private List<Book> listItem;

        public TabViewUi()
        {
            listItem = new List<Book>
            {
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"},
                new Book {Title = "Test string"}
            };
            InitializeComponent();

            TabView tabView = TabView;
            var tabFlower = new TabViewItem { Header = "Flower" };
            tabView.TabItems.Add(tabFlower);
            // Tab content
            var txtDetails = new TextBlock();
            txtDetails.Text = "Lorem Ipsum";
            tabFlower.Content = txtDetails;
        }
    }
}