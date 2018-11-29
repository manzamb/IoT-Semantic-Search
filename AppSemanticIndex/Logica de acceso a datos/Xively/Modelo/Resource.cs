namespace AppSemanticIndex
{
    public class Resource
    {
        private int feedId;
        private string datastreamId;

        public int Feed_Id
        {
            get { return feedId; }
            set { feedId = value; }
        }
        
        public string Datastream_Id
        {
            get { return datastreamId; }
            set { datastreamId = value; }
        }

        public bool equals(object obj)
        {
            if (obj == null)
		    {
			    return false;
		    }

		    if (this == obj)
		    {
			    return true;
		    }
            return true;
        }

  
        public Resource(int feedId, string datastreamId)
        {
            this.feedId = feedId;
            this.datastreamId = datastreamId;
        }
    }
}
