using Dapper.Contrib.Extensions;
using Overhaul.Data.Attributes;

namespace Overhaul.Data
{
    public sealed class SqlSchema
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ClassType { get; set; }

        public string Columns { get; set; }

        // Add a more secure way if testing by having a way to delete or modify a collection of this types 
        // Seperate test from start up code

    }
}
