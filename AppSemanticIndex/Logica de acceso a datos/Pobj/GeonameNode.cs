using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AppSemanticIndex.Pobj
{
    [XmlRootAttribute("GeonameNode", Namespace = "AppSemanticIndex", IsNullable = false)]
    [Serializable]
    public class GeonameNode
    {
        public string ToponymName;              //Geonames: ToponymName             
        public string Nombre_lugar;             //Geonames: name
        public string geonameId;                //Geonames: geonameId   
        public string Longitud;                 //Geonames: lng
        public string Latitud;                  //Geonames: lat
        public string Codigo_pais;              //Geonames: countryCode
        public string Nombre_pais;              //Geonames: countryName
        public string fcl;                      //Geonames: fcl
        public string fcode;                    //Geonames: fcode

        //Propiedades adicionadas para la aplicación
        public string Nombre_lugar_usuario;     //Corresponde la nobre de location en el sensor
        public string Nombre_lugar_Jerarquico;  //Se coloca la jerarquia de lugares desde Geonames
        
        public GeonameNode()
        {
        }

    }
}
