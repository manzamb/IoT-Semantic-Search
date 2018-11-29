using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Globalization;
using srvIdexSemanticIoT;

/// <summary>
/// Descripción breve de BiotipoManager
/// </summary>
public class BiotipoManager
{
    //Variable publica del biotipo que maneja
    public Biotipo bio;

    //Para las conversiones internacionales
    private CultureInfo culture = CultureInfo.InvariantCulture;

    public BiotipoManager()
	{
		Biotipo bio = new Biotipo();
	}

    ///<summary>
    ///Esta función recibe un Biotipo (lugar). Consulta la ontología por las variables que miden el biotipo y posteriormente
    ///consulta el indice por cada variable con el fin de seleccionar los posibles sensores que pueden medir dicha información
    ///y asi poder desplegarla en un servicio de aplicación.
    ///</summary>
    ///<returns>
    ///Biotipo con la información medioambiental.
    /// </returns>
    /// <param name="nodoBiotipo">Es el biotipo que ha sido identificado y se desea su información</param>
    /// <param name="radio">Radio que define el área de acción del Biotipo</param>
    /// <param name="idioma">Idioma de preferencia del usuario. Por defecto busca Español</param>
    public Biotipo ObtenerInfoBiotipoMedioAmbiente(GeonameNode nodoBiotipo, string radio, string idioma)
    {
        //Objeto del negocio
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();
 
        //Biotipo generado para presentarlo al usuario
        Biotipo bio = new Biotipo();

        /****************** Obtenemos los Datos Generales del Biotipo **********************/
        bio.GeonameId = nodoBiotipo.geonameId;
        bio.name = nodoBiotipo.Nombre_lugar;
        bio.CountryName = nodoBiotipo.Nombre_pais;
        bio.CountryCode = nodoBiotipo.Codigo_pais;
        bio.AreaImpacto.lat = nodoBiotipo.Latitud;
        bio.AreaImpacto.lon = nodoBiotipo.Longitud.ToString();
        bio.AreaImpacto.Area = radio;

        /****************** Buscamos las variables de cada caracteristica desde la ontologia y Relacionan Sensores **********************/
        //Conceptos retornados de cada concepto
        List<string> conceptosE = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesE = new List<Variable>();

        //vaiables del Edafotopo
        string conceptoBuscar = "Edafotopo";
        conceptosE = sm.RetornarConceptos(conceptoBuscar,idioma);
        if(conceptosE != null)
            foreach(string concepto in conceptosE)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que la pueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesE.Add(variableMedir);
                }
            }
        bio.Edafotopo.Variables=variablesE;

        //vaiables del Hidrotopo
        //Conceptos retornados de cada concepto
        List<string> conceptosH = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesH = new List<Variable>();

        conceptoBuscar = "Hidrotopo";
        conceptosH = sm.RetornarConceptos(conceptoBuscar,idioma);
        if (conceptosH != null)
            foreach(string concepto in conceptosH)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que lapueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesH.Add(variableMedir);
                }
            }
        bio.Hidrotopo.Variables=variablesH;

        //vaiables del Climátopo
        //Conceptos retornados de cada concepto
        List<string> conceptosC = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesC = new List<Variable>();

        conceptoBuscar = "Climátopo";
        conceptosC = sm.RetornarConceptos(conceptoBuscar,idioma);
        if (conceptosC != null)
            foreach(string concepto in conceptosC)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que lapueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesC.Add(variableMedir);
                }
            }
        bio.Climatopo.Variables=variablesC;

        /****************** Los valores de referencia se los deja,mos al usuario **********************/
        return bio;
    }

    public Biotipo ObtenerInfoBiotipoContaminacion(GeonameNode nodoBiotipo, string radio, string idioma)
    {
        //Objeto del negocio
        ServiciosMedioAmbientales sm = new ServiciosMedioAmbientales();

        //Biotipo generado para presentarlo al usuario
        Biotipo bio = new Biotipo();

        /****************** Obtenemos los Datos Generales del Biotipo **********************/
        bio.GeonameId = nodoBiotipo.geonameId;
        bio.name = nodoBiotipo.Nombre_lugar;
        bio.CountryName = nodoBiotipo.Nombre_pais;
        bio.CountryCode = nodoBiotipo.Codigo_pais;
        bio.AreaImpacto.lat = nodoBiotipo.Latitud;
        bio.AreaImpacto.lon = nodoBiotipo.Longitud.ToString();
        bio.AreaImpacto.Area = radio;

        /****************** Buscamos las variables de cada caracteristica desde la ontologia y Relacionan Sensores **********************/
        //Conceptos retornados de cada concepto
        List<string> conceptosE = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesE = new List<Variable>();

        //vaiables del Aire
        string conceptoBuscar = "Contaminación del Aire";
        conceptosE = sm.RetornarConceptos(conceptoBuscar, idioma);
        if (conceptosE != null)
            foreach (string concepto in conceptosE)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que la pueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesE.Add(variableMedir);
                }
            }
        bio.ContaminacionAire.Variables = variablesE;

        //vaiables del Suelo
        //Conceptos retornados de cada concepto
        List<string> conceptosH = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesH = new List<Variable>();

        conceptoBuscar = "Contaminación del Suelo";
        conceptosH = sm.RetornarConceptos(conceptoBuscar, idioma);
        if (conceptosH != null)
            foreach (string concepto in conceptosH)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que lapueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesH.Add(variableMedir);
                }
            }
        bio.ContaminacionSuelo.Variables = variablesH;

        //vaiables del Agua
        //Conceptos retornados de cada concepto
        List<string> conceptosC = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesC = new List<Variable>();

        conceptoBuscar = "Contaminación del Agua";
        conceptosC = sm.RetornarConceptos(conceptoBuscar, idioma);
        if (conceptosC != null)
            foreach (string concepto in conceptosC)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que lapueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesC.Add(variableMedir);
                }
            }
        bio.ContaminacionAgua.Variables = variablesC;


        //Conceptos retornados de cada concepto
        List<string> conceptosS = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesS = new List<Variable>();

        //vaiables del Sonora
        conceptoBuscar = "Contaminación Sonora";
        conceptosS = sm.RetornarConceptos(conceptoBuscar, idioma);
        if (conceptosS != null)
            foreach (string concepto in conceptosS)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que la pueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesS.Add(variableMedir);
                }
            }
        bio.ContaminacionSonora.Variables = variablesS;

        //vaiables del Termica
        //Conceptos retornados de cada concepto
        List<string> conceptosT = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesT = new List<Variable>();

        conceptoBuscar = "Contaminación Térmica";
        conceptosT = sm.RetornarConceptos(conceptoBuscar, idioma);
        if (conceptosT != null)
            foreach (string concepto in conceptosT)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que lapueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesT.Add(variableMedir);
                }
            }
        bio.ContaminacionTermica.Variables = variablesT;

        //vaiables del Visual
        //Conceptos retornados de cada concepto
        List<string> conceptosV = new List<string>();
        //Variable relacionadas a buscar mediciones
        List<Variable> variablesV = new List<Variable>();

        conceptoBuscar = "Contaminación Visual";
        conceptosV = sm.RetornarConceptos(conceptoBuscar, idioma);
        if (conceptosV != null)
            foreach (string concepto in conceptosV)
            {
                if (concepto != conceptoBuscar)
                {
                    Variable variableMedir = new Variable();
                    variableMedir.Id = concepto;
                    //Buscar Sensores que lapueden medir
                    variableMedir.sensores = sm.RetornarMapaLugar(bio.AreaImpacto.lat, bio.AreaImpacto.lon, concepto, idioma, radio);
                    variablesV.Add(variableMedir);
                }
            }
        bio.ContaminacionVisual.Variables = variablesV;

        /****************** Los valores de referencia se los deja,mos al usuario **********************/
        return bio;
    }
}