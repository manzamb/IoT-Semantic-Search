namespace AppSemanticIndex
{
    public class Datastream
    {
        public string feedid;   //Este campo es solo con propositos de presentación de datos
        public string id;
        public string current_value;
        public string at;
        public string max_value;
        public string min_value;
        public string[] tags;
        public Unit unit;
        public Datapoint[] datapoints;

        public Datastream()
        {
            unit = new Unit();
        }
    }
}
