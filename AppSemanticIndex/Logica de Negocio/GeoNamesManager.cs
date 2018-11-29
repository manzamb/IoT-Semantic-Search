using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using AppSemanticIndex.Pobj;
using NGeo.GeoNames;
using System.Globalization;

namespace AppSemanticIndex
{
    class GeoNamesManager
    {
        private static readonly string UserName = ConfigurationManager.AppSettings["GeoNamesUserName"];
        private CultureInfo culture = CultureInfo.InvariantCulture;

        //Este método obtiene todos los registros de geonames dado un lugar espécifico en forma Like
        public List<GeonameNode> GeoNames_SearchByName(string lugar)
        {
            List<GeonameNode> gnodes = new List<GeonameNode>();

            using (var geoNames = new GeoNamesClient())
            {
                var resultados = geoNames.Search(new SearchOptions(SearchType.Name, lugar)
                {
                    UserName = UserName
                });
                
                foreach (Toponym res in resultados)
                {
                    gnodes.Add(parseToponymToGeonameNode(res));
                }
            }
            return gnodes;
        }

        //Este método obtiene todos los registros de geonames dado un lugar espécifico
        public List<GeonameNode> GeoNames_SearchByNameEquals(string lugar)
        {
            List<GeonameNode> gnodes = new List<GeonameNode>();

            using (var geoNames = new GeoNamesClient())
            {
                var resultados = geoNames.Search(new SearchOptions(SearchType.NameEquals, lugar)
                {
                    UserName = UserName
                });

                foreach (Toponym res in resultados)
                {
                    gnodes.Add(parseToponymToGeonameNode(res));
                }
            }
            return gnodes;
        }

        //Este método obtiene el Lugar más cercano a las Coordenadas enviadas. 
        //Como no se especifica radio retorna un solo lugar
        public GeonameNode GeoNames_FindNearbyPlaceName(string Latitud, string Longitud)
        {
            List<GeonameNode> gnodes = new List<GeonameNode>();
            CultureInfo culture;
            culture = CultureInfo.InvariantCulture;

            using (var geoNames = new GeoNamesClient())
            {
                var finder = new NearbyPlaceNameFinder
                {
                    Latitude = Convert.ToDouble(Latitud, culture),
                    Longitude = Convert.ToDouble(Longitud, culture),
                    UserName = UserName,
                };
                var resultados = geoNames.FindNearbyPlaceName(finder);

                if(resultados!= null)
                    foreach (Toponym res in resultados)
                    {
                        gnodes.Add(parseToponymToGeonameNode(res));
                    }
            }
            if (gnodes == null)
                return null;
            else
                return gnodes[0];
        }

        //Este método obtiene el Lugar más cercano a las Coordenadas enviadas. 
        //Adicionando la jerarquía de lugares geográficos
        public List<GeonameNode> GeoNames_ExtendedFindNearby(string Latitud, string Longitud)
        {
            List<GeonameNode> gnodes = new List<GeonameNode>();

            using (var geoNames = new GeoNamesClient())
            {
                var finder = new NearbyPlaceNameFinder
                {
                    Latitude = Convert.ToDouble(Latitud, culture),
                    Longitude = Convert.ToDouble(Longitud, culture),
                    UserName = UserName,
                };
                var resultados = geoNames.FindNearbyPlaceName(finder);
                //var resultados = geoNames.FindNearbyPlaceName(finder);

                if (resultados != null)
                    foreach (Toponym res in resultados)
                    {
                        gnodes.Add(parseToponymToGeonameNode(res));
                    }

                if (gnodes.Count > 0)
                    gnodes = GeoNames_Hierarchy(Convert.ToInt32((gnodes[0].geonameId), culture));
                else
                    return null;
            }
            if (gnodes == null)
                return null;
            else
                return gnodes;
        }

        //Esta función recibe un GeonameId y devuelve la lista Jerárquica de
        //lugares en los que se encuentra
        public List<GeonameNode> GeoNames_Hierarchy(int GeonameId)
        {
            List<GeonameNode> gnodes = new List<GeonameNode>();
            string jerarquia = string.Empty;

            using (var geoNames = new GeoNamesClient())
            {
                int geoNameId = GeonameId;
                var resultados = geoNames.Hierarchy(geoNameId, UserName, ResultStyle.Full);

                if (resultados != null)
                    foreach (Toponym res in resultados)
                    {
                        gnodes.Add(parseToponymToGeonameNode(res));
                    }

                //Obtenemos la jerarquía en un solo campo
                foreach (GeonameNode gnode in gnodes)
                {
                    //Hereda al siguiente su nombre
                    if(string.IsNullOrEmpty(jerarquia))
                        jerarquia = gnode.Nombre_lugar;
                    else
                        jerarquia = jerarquia + "," + gnode.Nombre_lugar;
                    
                    //Llena el nodo y propiedad correspondiente
                    gnode.Nombre_lugar_Jerarquico = jerarquia;
                }
            }
            return gnodes;
        }

        public double VerificarSensorenLugar(double latitud, double longitud, double LatSensor, double LonSensor, double radio, DistanceType dt)
        {
            double result = DistanciaDosPuntos(latitud, longitud, LatSensor, LonSensor, dt);

            if (result <= radio)
                return result;
            else
                return 0;
        }

        public double DistanciaDosPuntos(double lat1, double long1, double Lat2, double Lon2, DistanceType dt)
        {
            //Creamos los puntos
            Position punto1 = new Position();
            punto1.Latitude = lat1;
            punto1.Longitude = long1;
            Position punto2 = new Position();
            punto2.Latitude = Lat2;
            punto2.Longitude = Lon2;

            //Calulamos la distancia
            Haversine hv = new Haversine();
            return hv.Distance(punto1, punto2, dt);
        }

        //Permite convertir un Toponym en GeonameNode
        private GeonameNode parseToponymToGeonameNode(Toponym tp)
        {
            GeonameNode gnode = new GeonameNode();

            gnode.ToponymName = tp.ToponymName;
            gnode.Nombre_lugar = tp.Name;
            gnode.Latitud = tp.Latitude.ToString(culture);
            gnode.Longitud = tp.Longitude.ToString(culture);
            if(tp.CountryCode != null)
                gnode.Codigo_pais = tp.CountryCode.ToString();
            if (tp.CountryName != null)
                gnode.Nombre_pais = tp.CountryName.ToString();
            gnode.geonameId = tp.GeoNameId.ToString(culture);
            gnode.fcl = tp.FeatureClassCode.ToString();
            gnode.fcode = tp.FeatureCode.ToString();

            return gnode;
        }

    }
}
