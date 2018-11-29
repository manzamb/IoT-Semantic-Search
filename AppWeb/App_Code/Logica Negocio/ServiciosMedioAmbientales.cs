using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using srvIdexSemanticIoT;
using System.Globalization;
using System.Collections;
using System.Data;

/// <summary>
/// Esta clase crea una interfaz a Servicio Web del Indice Semántico, implementando la logica propia del negocio
/// </summary>
public class ServiciosMedioAmbientales
{
    #region "variables privadas y de inicialización de la clase"
    //Para las conversiones internacionales
    private CultureInfo culture = CultureInfo.InvariantCulture;

    //Crea una instancia a la variable Proxy de la logica del Negocio
    WSSemanticSearch srvIndex = null;

    #endregion

    #region "Constructor de Clase"
    public ServiciosMedioAmbientales()
	{
        //Definir el objeto de servicio web
        srvIndex = new WSSemanticSearch();
	}
    #endregion

    #region "Servicios del Indice Semántico"

    ///<summary>
    ///Esta función recibe un conjunto de caracteres a los cuales les hace un proceso de ampliación de consulta y
    ///posteriormente busca en el índice las coincidencias con respecto a toda la información que ha sido encontrada
    ///en el repositorio de información.
    ///</summary>
    ///<returns>
    ///DataSet con una lista de Sesnsores que cumplen con la consulta realizada. Los datos no tienen información de datastreams
    ///porque la información es recuperada directamente del índice
    /// </returns>
    /// <param name="consulta">Texto en lenguaje natural con la necesidad de información del usuario</param>
    /// <param name="idioma">Idioma preferido de consulta seleccionado por el usuario</param>
    public DataSet Buscar(string consulta, string idioma)
    {
            return srvIndex.Buscar(consulta, idioma);
    }

    ///<summary>
    ///Esta función recibe un conjunto de caracteres a los cuales les hace un proceso de ampliación de consulta y
    ///posteriormente busca en el índice las coincidencias con respecto a toda la información que ha sido encontrada
    ///en el repositorio de información.
    ///</summary>
    ///<returns>
    ///Liata de FeedXively o lista de Sesnsores que cumplen con la consulta realizada. Adiferecncia de Buscar
    ///este procedimiento retorna todos los metadatos de Feed incluyendo dtastreams.
    /// </returns>
    /// <param name="consulta">Texto en lenguaje natural con la necesidad de información del usuario</param>
    /// <param name="idioma">Idioma preferido de consulta seleccionado por el usuario</param>
    public List<FeedXively> BuscarFeeds(string consulta, string idioma)
    {
        FeedXively[] feedstmp = srvIndex.BuscarFeeds(consulta, idioma);
        //List<FeedXively> feedtmplist = new List<FeedXively>();
        //feedtmplist.AddRange(feedstmp);
        return feedstmp.Cast<FeedXively>().ToList();
        //return feedtmplist;
    }

    ///<summary>
    ///Esta función recibe la consulta y la expande con respecto a la ontologia cargada en el servicio. Para ello
    ///Identifica de los términos de la copnsulta los conceptos y extrae sus conceptos hijos o relacionados de primer nivel.
    ///Los conceptos que no encuentra, hace una expanción de sinonimos con wordnet y finalmente añade los términos no
    ///identificados al final de la consulta. Con los conceptos identificados realiza un procesos de similitud semantica
    ///para establecer su importancia en la consulta.
    ///</summary>
    ///<returns>
    ///Cadena con la consulta expandida en un formato más accesible a los buscadores.
    /// </returns>
    /// <param name="consulta">Corresponde a la consulta original del usuario</param>
    /// <param name="idioma">Idioma de preferencia del usuario. Por defecto busca Español</param>
    public string ExpandirConsulta(string consulta, string idioma)
    {
        return srvIndex.ExpandirConsultaConceptosOntologia(consulta, idioma);
    }

    ///<summary>
    ///Esta función recibe un Biotipo(lugar) en términos de latitud y longitud, una consulta (buscar)  y devuelve la información
    ///relacionada a los sensores que tienen influencia en dicho lugar (determinado por el radio) y sus relaciones a
    ///conceptos medioambientales. El idioma y el radio son opcionales. Por defecto busca en: Español y 100 km. 
    ///</summary>
    ///<returns>
    ///DataSet con una lista de Sensores que estan en el área de onfluencia circular del biotipo
    /// </returns>
    /// <param name="lat">Latitud del lugar en el cual se buscarna los sensores</param>
    /// <param name="lng">Longitud del lugar en el cual se buscarna los sensores</param>
    /// <param name="dtsResultados">Dataset con los sensores que se encontraron en la consulta del usuario</param>
    /// <param name="idioma">Idioma de preferencia del usuario. Por defecto busca Español</param>
    /// <param name="radio">Área circular al rededor del punto geografico enviado en kilometros en los cuales se buscaran los sensores</param>
    public List<FeedXively> RetornarMapaLugar(string lat, string lng, string consulta, string idioma, string radio)
    {
        List<FeedXively> feedList = new List<FeedXively>();

        feedList.AddRange(srvIndex.RetornarMapaLugar(Convert.ToDouble(lat, culture),
                                           Convert.ToDouble(lng, culture),
                                           consulta, idioma, Convert.ToDouble(radio, culture)));
        return feedList;
    }

    ///<summary>
    ///Esta función recibe un Biotipo(lugar) en términos de latitud y longitud, una consulta (buscar)  y devuelve la información
    ///relacionada a los sensores que tienen influencia en dicho lugar (determinado por el radio) y sus relaciones a
    ///conceptos medioambientales. El idioma y el radio son opcionales. Por defecto busca en: Español y 100 km.
    ///La variante con respecto al anteriopr es que busca en Xively los datapoints
    ///</summary>
    ///<returns>
    ///DataSet con una lista de Sensores que estan en el área de onfluencia circular del biotipo
    /// </returns>
    /// <param name="lat">Latitud del lugar en el cual se buscarna los sensores</param>
    /// <param name="lng">Longitud del lugar en el cual se buscarna los sensores</param>
    /// <param name="consulta">Consulta del usuario</param>
    /// <param name="fechaFin">Fecha inicial del rango de datapoint a recuperar</param>
    /// <param name="fechaInicio">FEcha Final del rango de datapoint a recuperar</param>
    /// <param name="idioma">Idioma de preferencia del usuario. Por defecto busca Español</param>
    /// <param name="radio">Área circular al rededor del punto geografico enviado en kilometros en los cuales se buscaran los sensores</param>
    public List<FeedXively> RetornarMapaLugarDatapoints(string lat, string lng, string consulta, DateTime fechaInicio, DateTime fechaFin, string idioma, string radio)
    {
        List<FeedXively> feedList = new List<FeedXively>();

        srvIndex.Timeout = -1;
        feedList.AddRange(srvIndex.RetornarMapaLugarDatapoints(Convert.ToDouble(lat, culture),
                                                               Convert.ToDouble(lng, culture),
                                                               consulta,
                                                               fechaInicio,
                                                               fechaFin,
                                                               idioma, 
                                                               Convert.ToDouble(radio, culture)));
        return feedList;
    }

    ///<summary>
    ///Esta función recibe un Biotipo(lugar) en términos de latitud y longitud, una lista de sensores  y devuelve la información
    ///relacionada a los sensores que tienen influencia en dicho lugar (determinado por el radio) y sus relaciones a
    ///conceptos medioambientales. El idioma y el radio son opcionales. Por defecto busca en: Español y 100 km. 
    ///</summary>
    ///<returns>
    ///DataSet con una lista de Sensores que estan en el área de onfluencia circular del biotipo
    /// </returns>
    /// <param name="lat">Latitud del lugar en el cual se buscarna los sensores</param>
    /// <param name="lng">Longitud del lugar en el cual se buscarna los sensores</param>
    /// <param name="dtsResultados">Dataset con los sensores que se encontraron en la consulta del usuario</param>
    /// <param name="idioma">Idioma de preferencia del usuario. Por defecto busca Español</param>
    /// <param name="radio">Área circular al rededor del punto geografico enviado en kilometros en los cuales se buscaran los sensores</param>
    public List<FeedXively> RetornarMapaLugarListaSensores(string lat, string lng, List<FeedXively> feedList, string idioma, string radio)
    {
        FeedXively[] feedsarr = feedList.ToArray();
        feedsarr = srvIndex.RetornarMapaLugarListaSensores(Convert.ToDouble(lat, culture),
                                                       Convert.ToDouble(lng, culture),
                                                       feedsarr, idioma,
                                                       Convert.ToDouble(radio, culture));
        return feedsarr.Cast<FeedXively>().ToList();
    }

    ///<summary>
    ///Esta función busca los Biotipos realcionados a una lista de sensores encontrados en una busqueda realizada al indice.
    ///</summary>
    ///<returns>
    ///DataSet con una lista de Sesnsores que cumplen con la consulta realizada
    /// </returns>
    /// <param name="ListaSensoresEncontrados">Lista de sensores encontrados despues de la consulta de usuario</param>
    public List<GeonameNode> ObtenerBiotiposConsulta(List<GeonameNode> ListaSensoresEncontrados)
    {
        //Serializar la lista de GeonameNodes
        string xmlListGeonodes = ListaSensoresEncontrados.SerializarToXml();

        //Obtenemos la División Administrativa de Primer orden para la lsita de sensores
        //que tienene localización
        srvIndex.Timeout = -1;
        xmlListGeonodes = srvIndex.ObtenerListaLugarCoordenadas_AM1(xmlListGeonodes);

        //Desserializamos los objetos retornados
        List<GeonameNode> gnCercanos = xmlListGeonodes.DeserializarTo<List<GeonameNode>>();
        
        return gnCercanos;
    }

    ///<summary>
    ///Esta función recibe un término y lo compara en la ontología con el fin de establecer si es un concepto. 
    ///Si es concepto retorna los conceptos más relacionados al mismo (relaciones directas a otros conceptos), 
    ///desde el punto de vista del dominio de la ontología cargada.
    ///</summary>
    ///<returns>
    ///Lista de conceptos relacionados semánticamente al concepto buscado
    /// </returns>
    /// <param name="concepto">Conncepto que se esta buscando en la ontología</param>
    /// <param name="idioma">Idioma de preferencia del usuario.</param>
    public List<string> RetornarConceptos(string concepto, string idioma)
    {
        string res = srvIndex.RetornarConceptos(concepto, idioma);
        return res.DeserializarTo<List<string>>();
    }

    ///<summary>
    ///Esta función retorna un FeedXively con lod datapoints correspondientes entre un rago de tiempo
    ///</summary>
    ///<returns>
    ///Feed completo
    /// </returns>
    /// <param name="concepto">Conncepto que se esta buscando en la ontología</param>
    /// <param name="idioma">Idioma de preferencia del usuario.</param>
    public FeedXively HistoricosSensor(string feedID, DateTime fechaInicio, DateTime fexhaFin)
    {
        FeedXively fx = new FeedXively();

        fx = srvIndex.RetornarDatosSensor(feedID, fechaInicio, fexhaFin);

        return fx;
    }

    //Aqui vienen los demás servicios del negocio
    //5.	TendenciaVariableAmbiental (variable medioambiental): La tendencia debe implementar un sistema de análisis correlacional básico con el fin de proveer información de tendencias de variables. Se utilizará lo más básico pero aquí se pueden colocar análisis más elaborados que se dejan para futuros estudios.
    //6.	VariablesDiferentesBiotipo (lugar): Este método debe comparar los valores de referencia almacenados en el Biotipo de ciertos lugares y posteriormente con las mediciones de los sensores establecer si se encuentran en los rangos establecidos o se han salido de sus valores esperados. Para esto hay que buscar la información de referencia y poblar la ontología con la misma en un lugar seleccionados para demostrar sus posibilidades de uso.
    //7.	CausandoDañoAmbiental (lugar): Este método devuelve la información de los posibles agentes contaminantes que están afectando cierto lugar. Para esto debe tomar los sensores de dicho lugar y posteriormente utilizar la función de VariablesDiferentesBiotipo, las variable identificadas se relacionan a sus agentes contaminantes y se retorna una respuesta en una estructura de conceptos provista por la ontología en la cual se identifican dichos contaminantes.
    //8.	DañosAmbientales (lugar, AgentesContaminantes): A partir de un conjunto de agentes contaminantes y su magnitud se desarrolla un modelo matemático sencillo para relacionarlo a un daño medioambiental. La respuesta son los posibles daños causados el ecosistema por estos agentes.



    #endregion

    #region "Servicios del Administrador"
    ///<summary>
    ///Esta función llama al servicio que crea el índice semántico. Previamente debe haber cargado la ontología
    ///y configurado las variables de fuente de datos y tipo de lematizador. De lo contrario lo realizará con las
    ///preferencias actuales en el archivo de configuración del servicio web.
    ///</summary>
    ///<returns>
    ///Mensaje de confirmación de creación del índice
    /// </returns>
    public string CrearIndiceSemantico(string ontologia)
    {
        //Copiar la ontologia actual
        string nombreOntactual = ObtenerConfiguracion("FileOntology");

        //Se cambia el nombre de la Ontología antes de llamar el índice
        AsignarConfiguracion("FileOntology", ontologia);
            
        //Dado que es una operación larga se evita que termine por tiempo de espera
        srvIndex.Timeout = -1;

        //Se llama al servcio de crear el indice semántico
        string res = srvIndex.CrearIndiceSemantico(ontologia);

        //Restablecer Ontologia anterior
        if (res.Contains("Error") == true)
            AsignarConfiguracion("FileOntology", nombreOntactual);
        return res;
    }

    ///<summary>
    ///Esta función trae el valor de una clave del web.config del servicio web del índice semántico.
    ///</summary>
    ///<returns>
    ///Mensaje de confirmación de exito de la operación.
    /// </returns>
    /// <param name="key">Clave que se desea consultar</param>
    public string  ObtenerConfiguracion(string key)
    {
        return srvIndex.ObtenerConfiguracion(key);
    }

    ///<summary>
    ///Esta función asigna el valor de una clave del web.config del servicio web del índice semántico.
    ///</summary>
    ///<returns>
    ///Mensaje de confirmación de exito de la operación.
    /// </returns>
    /// <param name="key">Clave que se desea modificar</param>
    /// <param name="valor">Valor que se desea relacionar y almacenar de la clave especificada</param>
    public void AsignarConfiguracion(string key, string valor)
    {
        srvIndex.AsignarConfiguracion(key,valor);
    }

    ///<summary>
    ///Esta función toma la ontología especificada y la copia localmente en el servidor.
    ///</summary>
    ///<returns>
    ///Mensaje de confirmación de exito de la operación.
    /// </returns>
    /// <param name="UbicacionNombreOntologia">Ruta y nombre absolutos y locales de la ontología a ser cargada</param>
    public string CargarOntologia(string UbicacionNombreOntologia, byte[] info)
    {
        srvIndex.Timeout = -1;
        return srvIndex.CargarOntologia(UbicacionNombreOntologia, info);
    }

    /////<summary>
    /////Esta función toma la ontología especificada y la copia localmente en el servidor. Adicionalmente, 
    /////Carga a la base de datos del servicio los conceptos para su procesamiento posterior.
    /////</summary>
    /////<returns>
    /////Mensaje de confirmación de exito de la operación.
    ///// </returns>
    ///// <param name="UbicacionNombreOntologia">Ruta y nombre absolutos y locales de la ontología a ser cargada</param>
    //public string CargarOntologiaeIndexar(string UbicacionNombreOntologia)
    //{
    //    //Dado que es una operación larga se evita que termine por tiempo de espera
    //    srvIndex.Timeout = -1;
    //    return  srvIndex.CargarOntologiaeIndexar(UbicacionNombreOntologia);
    //}

    ///<summary>
    ///Esta función carga un feed de Xively de manera específica. Anotándo e indexandolo en el indice actual 
    ///Puede ser importante para indexar feeds que aun no han sido indexados por el servidor en su servicio de
    ///búsqueda a través del API.
    ///</summary>
    ///<returns>
    ///Mensaje de confirmación de exito de la operación.
    /// </returns>
    /// <param name="feedId">El código del feed provisto por Xively para identificar el dispositivo</param>
    public string CargarFeedXivelyBDD(string feedId)
    {
        //Dado que es una operación larga se evita que termine por tiempo de espera
        srvIndex.Timeout = -1;
        return srvIndex.CargarFeedXivelyBDD(feedId);
    }
    #endregion

    #region "Servicios de Evaluacion"
    /// <summary>
    /// Método que almacena una Consulta
    /// </summary>       
    /// <param name="myConsulta">Objeto de tipo Consulta que se va almacenar</param>
    /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
    public int SaveConsulta(string ConsultaOriginal, string ConsultaExpandida, DateTime FechaConsulta, string Usuario)
    {
        //Se llama al servcio de crear el indice semántico
        return srvIndex.SaveConsulta(ConsultaOriginal, ConsultaExpandida, FechaConsulta, Usuario);
    }

    /// <summary>
    /// Método que almacena una Calificacion
    /// </summary>       
    /// <param name="myCalificacion">Objeto de tipo Calificacion que se va almacenar</param>
    /// <returns>retorna 1 si la transaccion se realizó con exito, de lo contrario retorna 0</returns>
    public int SaveCalificacion(int ConsultaId, string feedId, int calificacion)
    {
        //Se llama al servcio de crear el indice semántico
        return srvIndex.SaveCalificacion(ConsultaId, feedId, calificacion);
    }
    #endregion

}