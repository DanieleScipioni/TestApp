using System.Collections.Generic;

namespace TestAppUWP.AppShell.Samples.Test
{
    public sealed partial class TestPage
    {
        private List<Book> listItem;

        public TestPage()
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

    internal class Book
    {
        public string Title;
    }
}
