using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for LasReadBook.xaml
    /// </summary>
    public partial class LastReadBook : Window
    {
        private string userName;
        private static readonly RegionEndpoint dynamoDBRegion = RegionEndpoint.USEast1;
        private readonly AmazonDynamoDBClient dynamoDBClient;
        private readonly BasicAWSCredentials credentials;
        DynamoDBContext context;

        private UserBook myCurrentLastBook = new UserBook();

        public LastReadBook(string username)
        {
            InitializeComponent();

            credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            dynamoDBClient = new AmazonDynamoDBClient(credentials, dynamoDBRegion);
            context = new DynamoDBContext(dynamoDBClient);

            userName = username;
        }

        private void btnLogo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("IEXPLORE.EXE", "http://www.centennialcollege.ca");
        }

        private async void frmLastReadBook_Loaded(object sender, RoutedEventArgs e)
        {
            // IEnumerable<UserBook> myReadingHistory = context.Query<UserBook>(userName);
            IEnumerable<UserBook> myReadingHistory = await context.QueryAsync<UserBook>(userName).GetRemainingAsync();

            if (myReadingHistory.Count() == 0)
            {
                AddToBookshelf addToBookshelf = new AddToBookshelf(userName);
                addToBookshelf.Show();
                // addToBookshelf.Show();
                this.Close();
                return;
            }

            myReadingHistory = myReadingHistory.OrderByDescending(s => s.LastAccessedBookDateTime);

            myCurrentLastBook = myReadingHistory.First<UserBook>();
            imgCoverPage.Source = new BitmapImage(new Uri(myReadingHistory.First<UserBook>().CoverPage, UriKind.Relative));
            txtTitle.Text = myReadingHistory.First<UserBook>().Title;
            txtBookDetails.Text = "ISBN: " + myReadingHistory.First<UserBook>().ISBN + "\n" + "Author: " + myReadingHistory.First<UserBook>().Author + "\n" + "Publisher: " + myReadingHistory.First<UserBook>().Publication + "\n\n" + "Synopsis: " + myReadingHistory.First<UserBook>().Synopsis;
        }

        private void Button_Click_Continue_Reading(object sender, RoutedEventArgs e)
        {
            PDFViewWindow pdfViewWindow = new PDFViewWindow(myCurrentLastBook);
            pdfViewWindow.Show();
            // pdfViewWindow.Show();
            this.Close();
        }

        private void Button_Click_Shop(object sender, RoutedEventArgs e)
        {
            Process.Start("IEXPLORE.EXE", "https://www.amazon.ca/s?k=books&ref=nb_sb_noss_2");
        }

        private void Button_Click_Donate(object sender, RoutedEventArgs e)
        {
            Process.Start("IEXPLORE.EXE", "https://tplfoundation.ca/donate-books/");
        }
    }
}
