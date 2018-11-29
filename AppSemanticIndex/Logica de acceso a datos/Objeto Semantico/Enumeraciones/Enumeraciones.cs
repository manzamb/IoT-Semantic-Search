namespace ObjetoSemantico
{
    //Esta enumeración es para el ObjectType de la clase Objeto
    public enum DObjectType { VirtualObject, PhysicalObject, NoSet };

    //Esta enumeración es para el Status de la clase Estados
    public enum DStatus { Draft, Final, Revised, NoSet };

    //Esta enumeración es para el Role de la clase Estados
    public enum DRole { Author, Publisher, Initiator, Terminator, Validator, Editor, Designer, Implementer, Provider, NoSet };

    //Esta enumeración es para el EstadoPlanificación de la clase Estados
    public enum DEstPlan{ Planificación, Programación, Ejecución, Análisis, NoSet };

    //Esta enumeración es para el TipoContexto de la clase Conocimientos
    public enum DContexto { Primario, Secundario, NoSet };

    //Esta enumeración es para el Kind de la clase Estructurales
    public enum DKind { HasPart, IsVersionOf, HasVersion, IsFormatOf, HasFormat, References, IsReferencedBy, IsBasedOn, IsBasisFor, Requires, IsRequiredBy, NoSet };

}