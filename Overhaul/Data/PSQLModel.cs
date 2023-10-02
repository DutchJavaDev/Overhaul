namespace Dbhaul.Data
{
    public sealed class PSQLModel
    {
        // Tablename
        public string Name { get; set; }

        // Columns (column1 datatype(lenght) column_constraint), seperated by comma's
        public string Columns { get; set; }

        // Table constraints? dunno
        public string TableConstraints { get; set; }


    }
}
