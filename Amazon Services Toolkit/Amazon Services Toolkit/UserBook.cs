using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Amazon_Services_Toolkit
{
    [DynamoDBTable("UserBookshelf")]
    public class UserBook : INotifyPropertyChanged
    {
        private bool toRemove;

        [DynamoDBHashKey]
        public string Username { get; set; }
        [DynamoDBRangeKey]
        public string LastAccessedBookDateTime { get; set; }
        [DynamoDBProperty("ISBN")]
        public string ISBN { get; set; }
        [DynamoDBProperty("Title")]
        public string Title { get; set; }
        [DynamoDBProperty("Author")]
        public string Author { get; set; }
        [DynamoDBProperty("Publication")]
        public string Publication { get; set; }
        [DynamoDBProperty("Synopsis")]
        public string Synopsis { get; set; }
        [DynamoDBProperty("CoverPage")]
        public string CoverPage { get; set; }
        [DynamoDBProperty("LastPageRead")]
        public string LastPageRead { get; set; }
        [DynamoDBProperty("TotalNumberPages")]
        public string TotalNumberPages { get; set; }
        [DynamoDBIgnore]
        public bool ToRemove
        {
            get
            {
                return toRemove;
            }
            set
            {
                toRemove = value;
                RaiseProperChanged();
            }
        }
        [DynamoDBIgnore]
        public string BookDetails { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaiseProperChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
