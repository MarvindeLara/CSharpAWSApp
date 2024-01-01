using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
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
using System.Windows.Shapes;

namespace Amazon_Services_Toolkit
{
    /// <summary>
    /// Interaction logic for DownloadFromS3Bucket.xaml
    /// </summary>
    public partial class DownloadFromS3Bucket : Window
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 s3Client;

        private List<string> buckets = new List<string>();
        private static string selectedBucket;

        public DownloadFromS3Bucket()
        {
            InitializeComponent();
            s3Client = new AmazonS3Client(GetBasicCredentials(), bucketRegion);

            cbxBuckets.ItemsSource = buckets;
        }

        private async void frmDownload_Loaded(object sender, RoutedEventArgs e)
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

        private async void Button_Click_DownloadFromS3Bucket(object sender, RoutedEventArgs e)
        {
            string fileName;
            string bucketName;
            string message;

            fileName = txtFileName.Text;
            bucketName = selectedBucket;

            try
            {
                await DownloadFile(bucketName, fileName);

                message = fileName + " was DOWNLOADED successfully.";
                MessageBox.Show(message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private static async Task DownloadFile (string bucketName, string fileName)
        {
            var downloadRequest = new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = $"C:\\Users\\Marvin\\Downloads\\{fileName}"
            };

            using (var transferUtility = new TransferUtility(s3Client))
            {
                await transferUtility.DownloadAsync(downloadRequest);
            }
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
    }
}
