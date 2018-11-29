using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSemanticIndex
{
    [Serializable]
    public class Feed
    {
        public string id;
        public string title;
        public bool Private;
        public string[] tags;
        public string description;
        public string feed;
        public string auto_feed_url;
        public Status status;
        public string updated;
        public string created;
        public string creator;
        public string version;
        public string website;
        public Datastream[] datastreams;      
        public Location location;
        private string titleHTML;
        private string uRLMostrar;

        public string TitleHTML
        {
            get 
            { 
                string creartitulo = string.Format("<a style=\"color: #336600; font-size:110%;\"  href=\"{1}\" >{0}</a>", title, URLMostrar);
                return creartitulo;
            }
            set
            {
                titleHTML = value;
            }

        }

        public string URLMostrar
        {
            get 
            { 
                string temp = this.feed;
                if (!string.IsNullOrEmpty(temp))
                {
                    temp = feed.Remove(feed.IndexOf("api."), 4);
                    temp = temp.Remove(temp.IndexOf("v2/"), 3);
                    temp = temp.Remove(temp.IndexOf(".json"), 5);
                }
                return temp;
            }
            set
            {
                uRLMostrar = value;
            }
        }

        //Constructor por defecto

        public Feed()
        {
            location = new Location();
        }
    }
}
