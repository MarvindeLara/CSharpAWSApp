using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon_Services_Toolkit
{
    [DynamoDBTable("Users")]
    public class User
    {
        [DynamoDBHashKey]
        public string Username { get; set; }
        [DynamoDBProperty("Password")]
        public string Password { get; set; }
    }
}
