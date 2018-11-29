namespace AppSemanticIndex
{
    public class ApiKey
    {
        private string apiKey;
        private string label;
        private bool isPrivateAccess;
        private Permission permissions;

        public bool private_access
        {
            get { return isPrivateAccess; }
            set { isPrivateAccess = value; }
        }

        public string api_key
        {
            get { return apiKey; }
            set { apiKey = value; }
        }

        public string getIdString()
        {
            return apiKey;
        }
    }
}
