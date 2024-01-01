using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
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
using System.Windows.Threading;

namespace Amazon_Services_Toolkit
{
    /// <summary>
    /// Interaction logic for CreateS3Bucket.xaml
    /// </summary>
    public partial class CreateS3Bucket : Window
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 s3Client;

        public CreateS3Bucket()
        {
            InitializeComponent();
            s3Client = new AmazonS3Client(GetBasicCredentials(), bucketRegion);
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

        private async void Button_Click_CreateBucket(object sender, RoutedEventArgs e)
        {
            string bucketName;
            BucketResponse responseCreate;
            ListBucketsResponse response;
            string message;

            bucketName = txtBucketName.Text;

            // Check if it contains uppercase characters
            if (bucketName.Any(char.IsUpper) || bucketName.Any(char.IsWhiteSpace))
            {
                MessageBox.Show("Bucket name INVALID.", "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            response = await GetBucketList();

            foreach (S3Bucket bucket in response.Buckets)
            {
                if (bucketName.Equals(bucket.BucketName))
                {
                    message = "Bucket " + bucket.BucketName + " was already CREATED.\nCreation date: " + bucket.CreationDate.ToLongDateString();
                    MessageBox.Show(message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            try
            {
                responseCreate = await CreateBucket(bucketName);

                message = "Bucket " + responseCreate.BucketName + " was CREATED successfully.\nRequest ID: " + responseCreate.RequestId;
                MessageBox.Show(message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "AWS S3", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        private static async Task<BucketResponse> CreateBucket(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            var response = await s3Client.PutBucketAsync(putBucketRequest);

            return new BucketResponse(bucketName, response.ResponseMetadata.RequestId);
        }

        private static async Task<ListBucketsResponse> GetBucketList()
        {
            ListBucketsResponse response = await s3Client.ListBucketsAsync();

            return response;
        }
    }
}
