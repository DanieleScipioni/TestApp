using System.Collections.Generic;

namespace TestAppUWP.AppShell.Samples.Test
{
    public sealed partial class TabViewToolkit
    {
        private List<Book> listItem;

        public TabViewToolkit()
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
        }
    }
}
