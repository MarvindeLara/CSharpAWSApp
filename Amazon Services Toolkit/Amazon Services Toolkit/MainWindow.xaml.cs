using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Amazon_Services_Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string userName;

        public MainWindow(string username)
        {
            InitializeComponent();
            mnuLogOut.Header = $"Log Out ({username})";
            userName = username;

            string code = "\nCODES: Amazon S3\n\n\n";
            code = code + "======== App.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\App.xaml.cs") + "\n";
            code = code + "======== LogIn.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\LogIn.xaml.cs") + "\n";
            code = code + "======== MainWindow.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\MainWindow.xaml.cs") + "\n";
            code = code + "======== BucketResponse.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\BucketResponse.cs") + "\n";
            code = code + "======== CreateS3Bucket.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\CreateS3Bucket.xaml.cs") + "\n";
            code = code + "======== DeleteS3Bucket.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\DeleteS3Bucket.xaml.cs") + "\n";
            code = code + "======== ObjectItem.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\ObjectItem.cs") + "\n";
            code = code + "======== UploadToS3Bucket.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\UploadToS3Bucket.xaml.cs") + "\n";
            code = code + "======== DownloadFromS3Bucket.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\DownloadFromS3Bucket.xaml.cs") + "\n";

            code = code + "CODES: Amazon DynamoDB\n\n\n";
            code = code + "======== Book.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\Book.cs") + "\n";
            code = code + "======== User.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\User.cs") + "\n";
            code = code + "======== UserBook.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\UserBook.cs") + "\n";
            code = code + "======== AddToBookshelf.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\AddToBookshelf.xaml.cs") + "\n";
            code = code + "======== ManageBookshelf.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\ManageBookshelf.xaml.cs") + "\n";
            code = code + "======== LastReadBook.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\LastReadBook.xaml.cs") + "\n";
            code = code + "======== PDFViewWindow.xaml.cs ========\n\n";
            code = code + File.ReadAllText("..\\..\\PDFViewWindow.xaml.cs") + "\n";

            txtEditor.Text = code;
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

        private void MenuItem_Click_CreateS3Bucket(object sender, RoutedEventArgs e)
        {
            CreateS3Bucket createS3Bucket = new CreateS3Bucket();
            createS3Bucket.ShowDialog();
            // createS3Bucket.Show();
        }

        private void MenuItem_Click_UploadToS3Bucket(object sender, RoutedEventArgs e)
        {
            UploadToS3Bucket uploadToS3Bucket = new UploadToS3Bucket();
            uploadToS3Bucket.ShowDialog();
            // uploadToS3Bucket.Show();
        }

        private void MenuItem_Click_DeleteS3Bucket(object sender, RoutedEventArgs e)
        {
            DeleteS3Bucket deleteS3Bucket = new DeleteS3Bucket();
            deleteS3Bucket.ShowDialog();
            // deleteS3Bucket.Show();
        }

        private void MenuItem_Click_DownloadFromS3Bucket(object sender, RoutedEventArgs e)
        {
            DownloadFromS3Bucket downloadFromS3Bucket = new DownloadFromS3Bucket();
            downloadFromS3Bucket.ShowDialog();
            // downloadFromS3Bucket.Show();
        }

        private void MenuItem_Click_LogOut(object sender, RoutedEventArgs e)
        {
            LogIn logIn = new LogIn();
            logIn.Show();
            this.Close();
        }

        private void Button_Click_Add_To_Bookshelf(object sender, RoutedEventArgs e)
        {
            AddToBookshelf addToBookshelf = new AddToBookshelf(userName);
            addToBookshelf.ShowDialog();
            // addToBookshelf.Show();
        }

        private void Button_Click_Manage_Bookshelf(object sender, RoutedEventArgs e)
        {
            ManageBookshelf manageBookshelf = new ManageBookshelf(userName);
            manageBookshelf.ShowDialog();
            // manageBookshelf.Show();
        }

        private void Button_Click_Last_Read_Book(object sender, RoutedEventArgs e)
        {
            LastReadBook lastReadBook = new LastReadBook(userName);
            lastReadBook.ShowDialog();
            // lastReadBook.Show();
        }
    }
}
