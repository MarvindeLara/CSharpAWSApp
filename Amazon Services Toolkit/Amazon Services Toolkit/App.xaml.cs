using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Amazon_Services_Toolkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly RegionEndpoint dynamoDBRegion = RegionEndpoint.USEast1;
        private readonly AmazonDynamoDBClient dynamoDBClient;
        private readonly BasicAWSCredentials credentials;
        private readonly DynamoDBContext context;

        private static string userTable = "Users";
        private static string userBookshelfTable = "UserBookshelf";
        private static string bookCatalogueTable = "BookCatalogue";

        public App()
        {
            InitializeComponent();

            credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            dynamoDBClient = new AmazonDynamoDBClient(credentials, dynamoDBRegion);
            context = new DynamoDBContext(dynamoDBClient);

            CreateUsersTable();
            CreateUserBookshelfTable();
            CreateUserBookCatalogueTable();
        }

        public async void CreateUsersTable()
        {
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = userTable,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Username",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Username",
                        KeyType = "HASH"
                    }
                },
                BillingMode = BillingMode.PROVISIONED,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            try
            {
                var response = await dynamoDBClient.CreateTableAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{userTable} was CREATED successfully.");

                    WaitTillTableCreated(dynamoDBClient, userTable, response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"MARVIN1: {e.ToString()}");
            }
        }

        public async void CreateUserBookshelfTable()
        {
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = userBookshelfTable,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Username",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "LastAccessedBookDateTime",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Username",
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "LastAccessedBookDateTime",
                        KeyType = "RANGE"
                    }
                },
                BillingMode = BillingMode.PROVISIONED,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            try
            {
                var response = await dynamoDBClient.CreateTableAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{userBookshelfTable} was CREATED successfully.");

                    WaitTillTableCreated(dynamoDBClient, userBookshelfTable, response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"MARVIN2: {e.ToString()}");
            }
        }

        public async void CreateUserBookCatalogueTable()
        {
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = bookCatalogueTable,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "ISBN",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "ISBN",
                        KeyType = "HASH"
                    }
                },
                BillingMode = BillingMode.PROVISIONED,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            try
            {
                var response = await dynamoDBClient.CreateTableAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{bookCatalogueTable} was CREATED successfully.");

                    WaitTillTableCreated(dynamoDBClient, bookCatalogueTable, response);

                    PopulateBookCatalogueTable();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"MARVIN3: {e.ToString()}");
            }
        }

        private static void WaitTillTableCreated(AmazonDynamoDBClient dynamoDBClient, string tableName, CreateTableResponse response)
        {
            var tableDescription = response.TableDescription;

            string status = tableDescription.TableStatus;

            while (status != "ACTIVE")
            {
                Thread.Sleep(1000);

                try
                {
                    var res = dynamoDBClient.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    status = res.Result.Table.TableStatus;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"MARVIN4: {e.ToString()}");
                }
            }
        }

        public async void PopulateBookCatalogueTable()
        {
            var bookBatch = context.CreateBatchWrite<Book>();

            Book Postalgia_SRS_v103 = new Book
            {
                ISBN = "111-11-11-1111",
                Title = "Postalgia SRS v1.03",
                Author = "Marvin de Lara",
                Publication = "WIMTACH",
                Synopsis = "Software Requirements Specification for POC - GUI - Network",
                CoverPage = ".\\assets\\Postalgia_SRS_v1.03.png",
                TotalNumberPages = "19"
            };

            Book SKILLS_AND_PORTFOLIO_SCREENSHOTS_v1 = new Book
            {
                ISBN = "222-22-22-2222",
                Title = "SKILLS AND PORTFOLIO SCREENSHOTS v1",
                Author = "Marvin de Lara",
                Publication = "Marvin de Lara",
                Synopsis = "Compilation of screenshots of applications for portfolio",
                CoverPage = ".\\assets\\SKILLS_AND_PORTFOLIO_SCREENSHOTS_v1.png",
                TotalNumberPages = "56"
            };

            Book Linear_Algebra_Custom_Textbook = new Book
            {
                ISBN = "333-33-33-3333",
                Title = "Linear Algebra Custom Textbook - 2017",
                Author = "Mandy Lam",
                Publication = "Centennial College",
                Synopsis = "Introduction to Linear Algebra",
                CoverPage = ".\\assets\\Linear Algebra Custom Textbook - 2017.png",
                TotalNumberPages = "89"
            };

            bookBatch.AddPutItems(new List<Book> { Postalgia_SRS_v103, SKILLS_AND_PORTFOLIO_SCREENSHOTS_v1, Linear_Algebra_Custom_Textbook });

            await bookBatch.ExecuteAsync();
        }
    }
}
