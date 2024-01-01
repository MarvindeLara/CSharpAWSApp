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
    /// Interaction logic for ManageBookshelf.xaml
    /// </summary>
    public partial class ManageBookshelf : Window
    {
        private string userName;
        private static readonly RegionEndpoint dynamoDBRegion = RegionEndpoint.USEast1;
        private readonly AmazonDynamoDBClient dynamoDBClient;
        private readonly BasicAWSCredentials credentials;
        DynamoDBContext context;

        private static ObservableCollection<UserBook> myBookshelf = new ObservableCollection<UserBook>();
        private static ObservableCollection<UserBook> myBookshelfShowAll = new ObservableCollection<UserBook>();

        private string message;

        public ManageBookshelf(string username)
        {
            InitializeComponent();

            credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            dynamoDBClient = new AmazonDynamoDBClient(credentials, dynamoDBRegion);
            context = new DynamoDBContext(dynamoDBClient);

            dtgMyBookshelf.ItemsSource = myBookshelf;

            userName = username;
        }

        private void btnLogo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("IEXPLORE.EXE", "http://www.centennialcollege.ca");
        }

        private async void frmManageBookshelf_Loaded(object sender, RoutedEventArgs e)
        {
            myBookshelf.Clear();
            myBookshelfShowAll.Clear();

            //IEnumerable<UserBook> myBookshelfAll = context.Scan<UserBook>(
            //    new ScanCondition("Username", ScanOperator.IsNotNull)
            //);

            //List<ScanCondition> scanConditions = new List<ScanCondition>();
            //scanConditions.Append(new ScanCondition("Username", ScanOperator.IsNotNull));
            //IEnumerable<UserBook> myBookshelfAll = await context.ScanAsync<UserBook>(scanConditions).GetRemainingAsync();

            IEnumerable<UserBook> myBookshelfAll = await context.QueryAsync<UserBook>(userName).GetRemainingAsync();

            myBookshelfAll = myBookshelfAll.OrderByDescending(s => s.LastAccessedBookDateTime);

            foreach (UserBook userBook in myBookshelfAll)
            {
                userBook.BookDetails = "Title: " + userBook.Title + "\n" + "ISBN: " + userBook.ISBN + "\n" + "Author: " + userBook.Author + "\n" + "Publisher: " + userBook.Publication + "\n\n" + "Synopsis: " + userBook.Synopsis + "\n";
                myBookshelf.Add(userBook);
                myBookshelfShowAll.Add(userBook);
            }
        }

        private void Button_Click_Search(object sender, RoutedEventArgs e)
        {
            bool found = false;
            UserBook bookSearched = new UserBook();

            foreach (UserBook book in myBookshelf)
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
                myBookshelf.Clear();
                myBookshelf.Add(bookSearched);
            }
        }

        private void Button_Click_Show_All(object sender, RoutedEventArgs e)
        {
            myBookshelf.Clear();
            foreach (UserBook book in myBookshelfShowAll)
            {
                myBookshelf.Add(book);
            }
        }

        private async void Button_Click_Removed(object sender, RoutedEventArgs e)
        {
            // IEnumerable<UserBook> userBookshelf = context.Query<UserBook>(userName);
            IEnumerable<UserBook> userBookshelf = await context.QueryAsync<UserBook>(userName).GetRemainingAsync();

            int countToRemove = 0;
            int countRemoved = 0;

            foreach (UserBook book in myBookshelf)
            {
                if (book.ToRemove == true)
                {
                    countToRemove++;

                    foreach (UserBook userBook in userBookshelf)
                    {
                        if (userBook.ISBN.Equals(book.ISBN))
                        {
                            countRemoved++;

                            // context.Delete(userBook);
                            await context.DeleteAsync(userBook);
                        }
                    }
                }
            }

            if (countToRemove == 0)
            {
                message = "Please select a book to remove from your bookshelf.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (countRemoved != 0)
            {
                message = "Selected book(s) removed from your bookshelf.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Information);

                myBookshelfShowAll.Clear();
                foreach (UserBook book in myBookshelf)
                {
                    if (book.ToRemove == false)
                        myBookshelfShowAll.Add(book);
                }

                myBookshelf.Clear();
                foreach (UserBook book in myBookshelfShowAll)
                    myBookshelf.Add(book);
            }
        }

        private async void Button_Click_Continue_Reading(object sender, RoutedEventArgs e)
        {
            // IEnumerable<UserBook> myReadingHistory = context.Query<UserBook>(userName);
            IEnumerable<UserBook> myReadingHistory = await context.QueryAsync<UserBook>(userName).GetRemainingAsync();

            if (dtgMyBookshelf.SelectedItem != null)
            {
                UserBook selectedBook = (UserBook)dtgMyBookshelf.SelectedItem;

                foreach (UserBook lastBook in myReadingHistory)
                {
                    if (lastBook.ISBN.Equals(selectedBook.ISBN))
                    {
                        PDFViewWindow pdfViewWindow = new PDFViewWindow(lastBook);
                        pdfViewWindow.Show();
                        // pdfViewWindow.Show();
                        this.Close();
                    }
                }
            }
        }
    }
}
