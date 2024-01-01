using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        private static readonly RegionEndpoint dynamoDBRegion = RegionEndpoint.USEast1;
        private readonly AmazonDynamoDBClient dynamoDBClient;
        private readonly BasicAWSCredentials credentials;
        private readonly DynamoDBContext context;

        private string message;

        public LogIn()
        {
            InitializeComponent();

            credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            dynamoDBClient = new AmazonDynamoDBClient(credentials, dynamoDBRegion);
            context = new DynamoDBContext(dynamoDBClient);
        }

        private async void Button_Click_LogIn(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text.Length == 0 || txtPasswordHidden.Password.Length == 0)
            {
                message = "Invalid credentials. Please check.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User userRetrieved = await context.LoadAsync<User>(txtUsername.Text);

            if (userRetrieved != null && userRetrieved.Username.Equals(txtUsername.Text) && userRetrieved.Password.Equals(txtPasswordHidden.Password))
            {
                MainWindow mainWindow = new MainWindow(userRetrieved.Username);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                message = "User NOT found. Please register an account.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            //IEnumerable<User> userAll = context.Scan<User>(
            //    new ScanCondition("Username", ScanOperator.Equal, txtUsername.Text)
            //);

            List<ScanCondition> scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Username", ScanOperator.Equal, txtUsername.Text));
            IEnumerable<User> userAll = await context.ScanAsync<User>(scanConditions).GetRemainingAsync();

            if (userAll.Count() != 0)
            {
                message = $"Username ({txtUsername.Text}) is already registered. Please use a different username.";
                MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User newUser = new User
            {
                Username = txtUsername.Text,
                Password = txtPasswordHidden.Password
            };

            await context.SaveAsync(newUser);

            message = "Registration successful.";
            MessageBox.Show(message, "Amazon Cloud Services", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
