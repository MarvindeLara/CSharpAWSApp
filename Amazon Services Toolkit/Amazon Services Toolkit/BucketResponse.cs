namespace Amazon_Services_Toolkit
{
    public class BucketResponse
    {
        private string bucketName;
        private string  requestId;

        public string BucketName
        {
            get
            {
                return bucketName;
            }
            set
            {
                bucketName = value;
            }
        }

        public string RequestId
        {
            get
            {
                return requestId;
            }
            set
            {
                requestId = value;
            }
        }

        public BucketResponse(string bucketName, string requestId)
        {
            BucketName = bucketName;
            RequestId = requestId;
        }
    }
}