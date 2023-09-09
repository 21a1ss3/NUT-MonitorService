namespace NUTMonitor.Esxi.Vmodl
{
    public enum VmodlToken
    {

        //(
        StartTypeName,
        TypeName,
        //)
        EndTypeName,
        //{
        StartObject,
        //}
        EndObject,
        PropertyName,
        //[
        StartArray,
        //,
        ArrayItemSeparator,
        //]
        EndArray,
        //'
        StartReference,
        ReferenceType,
        //:
        StartReferenceId,
        ReferenceId,
        //'
        EndReference,

        Unset,
        String,
        Integer,
        Decimal,
        Null,
        EOF,
        True,
        False,
    }
}
