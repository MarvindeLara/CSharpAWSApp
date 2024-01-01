using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Amazon_Services_Toolkit
{
    /// <summary>
    /// Interaction logic for AddToBookshelf.xaml
    /// </summary>
    public partial class AddToBookshelf : Window
    {
        private string userName;
        private static readonly RegionEndpoint dynamoDBRegion = RegionEndpoint.USEast1;
        private readonly AmazonDynamoDBClient dynamoDBClient;
        private readonly BasicAWSCredentials credentials;
        DynamoDBContext context;

        private static ObservableCollection<Book> bookCatalogue = new ObservableCollection<Book>();
        private static ObservableCollection<Book> bookCatalogueShowAll = new ObservableCollection<Book>();

        private string message;

        public AddToBookshelf(string username)
        {
            InitializeComponent();

            credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            dynamoDBClient = new AmazonDynamoDBClient(credentials, dynamoDBRegion);
            context = new DynamoDBContext(dynamoDBClient);

            dtgBookCatalogue.ItemsSource = bookCatalogue;

            userName = username;
        }

        private async void frmAddToBookshelf_Loaded(object sender, RoutedEventArgs e)
        {
            bookCatalogue.Clear();
            bookCatalogueShowAll.Clear();

            //IEnumerable<Book> bookCatalogueAll = context.Scan<Book>(
            //    new ScanCondition("ISBN", ScanOperator.IsNotNull)
            //);

            List<ScanCondition> scanConditions = new List<ScanCondition>();
            scanConditions.Append(new ScanCondition("ISBN", ScanOperator.IsNotNull));
            IEnumerable<Book> bookCatalogueAll = await context.ScanAsync<Book>(scanConditions).GetRemainingAsync();

            bookCatalogueAll = bookCatalogueAll.OrderBy(s => s.ISBN);

            foreach (Book book in bookCatalogueAll)
            {
                book.BookDetails = "Title: " + book.Title + "\n" + "ISBN: " + book.ISBN + "\n" + "Author: " + book.Author + "\n" + "Publisher: " + book.Publication + "\n\n" + "Synopsis: " + book.Synopsis;
                bookCatalogue.Add(book);
                bookCatalogueShowAll.Add(book);
            }
        }

        private void btnLogo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("IEXPLORE.EXE", "http://www.centennialcollege.ca");
        }

        private async void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            // IEnumerable<UserBook> userBookshelf = context.Query<UserBook>(userName);
            IEnumerable<UserBook> userBookshelf = await context.QueryAsync<UserBook>(userName).GetRemainingAsync();

            bool found = false;
            int countToAdd = 0;
            int countAdded = 0;

            foreach (Book book in bookCatalogue)
            {
                found = false;

                if (book.ToAdd == true)
                {
                    countToAdd++;

                    foreach (UserBook userBook in userBookshelf)
                    {
                        if (userBook.ISBN.Equals(book.ISBN))
                            found = true;
                    }

                    if (!found)
                    {
                        countAdded++;

                        DateTime now = DateTime.UtcNow;

                        UserBook userbook = new UserBook
                        {
                            Username = userName,
                            LastAccessedBookDateTime = now.ToString(),
                            ISBN = book.ISBN,
                            Title = book.Title,
                            Author = book.Author,
                            Publication = book.Publication,
                            Synopsis = book.Synopsis,
                            CoverPage = book.CoverPage,
                            LastPageRead = "1",
                            TotalNumberPages = book.TotalNumberPages
                        };

                        // context.Save(userbook);
                        await context.SaveAsync(userbook);

                        Thread.Sleep(1000);
                    }
                }
            }

            if (countToAdd == 0)
            {
                message = "Please select a book to add to your bookshelf.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (countAdded != 0)
            {
                message = "Selected book(s) added in your bookshelf.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (countToAdd != countAdded)
            {
                message = "Some selected book(s) are already added in your bookshelf.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_Show_All(object sender, RoutedEventArgs e)
        {
            bookCatalogue.Clear();
            foreach (Book book in bookCatalogueShowAll)
            {
                bookCatalogue.Add(book);
            }
        }

        private void Button_Click_Search(object sender, RoutedEventArgs e)
        {
            bool found = false;
            Book bookSearched = new Book();

            foreach (Book book in bookCatalogue)
            {
                if (book.Title.Contains(txtTitle.Text))
                {
                    found = true;
                    bookSearched = book;
                    break;
                }
            }

            if (found)
            {
                bookCatalogue.Clear();
                bookCatalogue.Add(bookSearched);
            }
        }
    }
}
