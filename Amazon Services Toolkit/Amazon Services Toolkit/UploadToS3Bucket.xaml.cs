using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Amazon_Services_Toolkit
{
    /// <summary>
    /// Interaction logic for UploadToS3Bucket.xaml
    /// </summary>
    public partial class UploadToS3Bucket : Window
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 s3Client;

        private static ObservableCollection<ObjectItem> objectsList = new ObservableCollection<ObjectItem>();
        private List<string> filenames = new List<string>();
        private List<string> buckets = new List<string>();
        private static string selectedBucket;

        public UploadToS3Bucket()
        {
            InitializeComponent();
            s3Client = new AmazonS3Client(GetBasicCredentials(), bucketRegion);

            dtgObjects.ItemsSource = objectsList;
            cbxBuckets.ItemsSource = buckets;
        }

        private async void frmUpload_Loaded(object sender, RoutedEventArgs e)
        {
            ListBucketsResponse response;
            String message;

            response = await GetBucketList();

            if (response.Buckets.Count == 0)
            {
                message = "Please create a bucket first.";
                MessageBox.Show(message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (S3Bucket bucket in response.Buckets)
            {
                buckets.Add(bucket.BucketName);
            }
        }

        private void btnLogo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("IEXPLORE.EXE", "http://www.centennialcollege.ca");
        }

        private static BasicAWSCredentials GetBasicCredentials()
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);

            var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

            return new BasicAWSCredentials(accessKeyID, secretKey);
        }

        private void Button_Click_Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            filenames.Clear();

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    filenames.Add(filename);
                }

                txtPath.Text = string.Join("; ", filenames);
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            String message;

            if (filenames.Count() == 0)
            {
                message = "Please select file(s) to upload.";
                MessageBox.Show(message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (string filename in filenames)
            {
                ObjectItem objectItem = new ObjectItem();

                objectItem.FileName = filename;
                objectItem.Status = "Upload Pending";

                objectsList.Add(objectItem);
            }

            filenames.Clear();
        }

        private static async Task<ListBucketsResponse> GetBucketList()
        {
            ListBucketsResponse response = await s3Client.ListBucketsAsync();

            return response;
        }

        private void cbxBuckets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedBucket = buckets[cbxBuckets.SelectedIndex];
        }

        private async void Button_Click_Upload(object sender, RoutedEventArgs e)
        {
            String message;

            if (objectsList.Count() == 0)
            {
                message = "Please select file(s) to upload.";
                MessageBox.Show(message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                await UploadFilesAsync();
                MessageBoxResult result = MessageBox.Show("File(s) UPLOADED successfully. Would you like to clear the form?", "AWS S3", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        objectsList.Clear();
                        txtPath.Text = "";
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private static async Task UploadFilesAsync()
        {
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);

                foreach (ObjectItem objectItem in objectsList)
                {
                    await fileTransferUtility.UploadAsync(objectItem.FileName, selectedBucket);
                    objectItem.Status = "Uploaded";
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
