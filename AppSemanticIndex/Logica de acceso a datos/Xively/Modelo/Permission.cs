namespace AppSemanticIndex
{
    public class Permission
    {
        private AccessMethod[] accessMethods;

        public AccessMethod[] access_methods
        {
            get { return accessMethods; }
            set { accessMethods = value; }
        }
        private string sourceIp;

        public string source_ip
        {
            get { return sourceIp; }
            set { sourceIp = value; }
        }

        private Resource[] resources;

        public Resource[] Resources
        {
            get { return resources; }
            set { resources = value; }
        }

        public Permission(string sourceIp, AccessMethod[] accessMethods, Resource[] resources)
        {
            this.sourceIp = sourceIp;
            this.accessMethods = accessMethods == null ? null : accessMethods;
            this.resources = resources == null ? null : resources;
        }
    }
}
