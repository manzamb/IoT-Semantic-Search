using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSemanticIndex
{
    public class Trigger
    {
        private TriggerType type;
        private int id;
        private string datastreamId;
        private int feedId;
        private int login;
        private string notifiedAt;
        private string thresholdValue;
        private Uri url;

        public string threshold_value
        {
            get { return thresholdValue; }
            set { thresholdValue = value; }
        }

        public string notified_at
        {
            get { return notifiedAt; }
            set { notifiedAt = value; }
        }

        public int user
        {
            get { return login; }
            set { login = value; }
        }

        public int environment_id
        {
            get { return feedId; }
            set { feedId = value; }
        }

        public string stream_id
        {
            get { return datastreamId; }
            set { datastreamId = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public TriggerType trigger_type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
