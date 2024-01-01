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
    /// Interaction logic for PDFViewWindow.xaml
    /// </summary>
    public partial class PDFViewWindow : Window
    {
        private UserBook myCurrentLastBook = new UserBook();
        private static readonly RegionEndpoint dynamoDBRegion = RegionEndpoint.USEast1;
        private readonly AmazonDynamoDBClient dynamoDBClient;
        private readonly BasicAWSCredentials credentials;
        DynamoDBContext context;

        // tweak for displaying the first loading of web page
        bool firstRefreshSkip = true;

        public PDFViewWindow(UserBook lastBook)
        {
            InitializeComponent();

            credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            dynamoDBClient = new AmazonDynamoDBClient(credentials, dynamoDBRegion);
            context = new DynamoDBContext(dynamoDBClient);

            myCurrentLastBook = lastBook;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;

            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;

            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void btnLogo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("IEXPLORE.EXE", "http://www.centennialcollege.ca");
        }

        private void frmPDFViewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mnuTitle.Header = "Title: " + myCurrentLastBook.Title;

            txtCurrentPage.Text = myCurrentLastBook.LastPageRead;
            txtTotalNumberPages.Text = myCurrentLastBook.TotalNumberPages;

            if (myCurrentLastBook.LastPageRead.Equals("1"))
            {
                webPDFViewer.Source = new Uri("https://ebookrepositorylab2.s3.amazonaws.com/" + myCurrentLastBook.Title + ".pdf");
            }
            else
            {
                webPDFViewer.Source = new Uri("https://ebookrepositorylab2.s3.amazonaws.com/" + myCurrentLastBook.Title + ".pdf" + "#page=" + myCurrentLastBook.LastPageRead);
            }
        }

        private async void Button_Click_Bookmark(object sender, RoutedEventArgs e)
        {
            string message;

            // context.Delete(myCurrentLastBook);
            await context.DeleteAsync(myCurrentLastBook);

            DateTime now = DateTime.UtcNow;

            myCurrentLastBook.LastAccessedBookDateTime = now.ToString();
            myCurrentLastBook.LastPageRead = txtCurrentPage.Text;

            // context.Save(myCurrentLastBook);
            await context.SaveAsync(myCurrentLastBook);

            message = $"Page {txtCurrentPage.Text} is bookmarked.";
            MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void txtCurrentPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    txtCurrentPage.Text = (Convert.ToInt32(txtCurrentPage.Text)).ToString();

                    if (Convert.ToInt32(txtCurrentPage.Text) < 0 || Convert.ToInt32(txtCurrentPage.Text) > Convert.ToInt32(myCurrentLastBook.TotalNumberPages))
                    {
                        txtCurrentPage.Text = "0";
                        return;
                    }
                    else
                    {
                        webPDFViewer.Source = new Uri("https://ebookrepositorylab2.s3.amazonaws.com/" + myCurrentLastBook.Title + ".pdf" + "#page=" + txtCurrentPage.Text);
                    }
                }
                catch (Exception error)
                {
                    txtCurrentPage.Text = "0";
                    Console.WriteLine($"MARVIN: {error.ToString()}");
                }
            }
        }

        private void Button_Click_Previous(object sender, RoutedEventArgs e)
        {
            txtCurrentPage.Text = (Convert.ToInt32(txtCurrentPage.Text) - 1).ToString();

            if (txtCurrentPage.Text.Equals("0"))
            {
                txtCurrentPage.Text = "1";
                return;
            }

            webPDFViewer.Source = new Uri("https://ebookrepositorylab2.s3.amazonaws.com/" + myCurrentLastBook.Title + ".pdf" + "#page=" + txtCurrentPage.Text);
        }

        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
            txtCurrentPage.Text = (Convert.ToInt32(txtCurrentPage.Text) + 1).ToString();

            if (txtCurrentPage.Text.Equals((Convert.ToInt32(myCurrentLastBook.TotalNumberPages) + 1).ToString()))
            {
                txtCurrentPage.Text = myCurrentLastBook.TotalNumberPages;
                return;
            }

            webPDFViewer.Source = new Uri("https://ebookrepositorylab2.s3.amazonaws.com/" + myCurrentLastBook.Title + ".pdf" + "#page=" + txtCurrentPage.Text);   
        }

        private void webPDFViewer_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (firstRefreshSkip)
            {
                firstRefreshSkip = false;
                return;
            }

            webPDFViewer.Refresh();
            // await Task.Delay(1000);
        }

        private async void frmPDFViewWindow_Closed(object sender, EventArgs e)
        {
            string message;

            if (!myCurrentLastBook.LastPageRead.Equals(txtCurrentPage.Text))
            {
                // context.Delete(myCurrentLastBook);
                await context.DeleteAsync(myCurrentLastBook);

                DateTime now = DateTime.UtcNow;

                myCurrentLastBook.LastAccessedBookDateTime = now.ToString();
                myCurrentLastBook.LastPageRead = txtCurrentPage.Text;

                // context.Save(myCurrentLastBook);
                await context.SaveAsync(myCurrentLastBook);

                message = $"Page {txtCurrentPage.Text} is bookmarked.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
