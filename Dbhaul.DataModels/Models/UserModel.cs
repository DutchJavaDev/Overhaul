using Dbhaul.Attributes;

namespace Dbhaul.DataModels.Models
{
    [TableName("tblUser")]
    public sealed class UserModel
    {
        [Id]
        public Guid? Id { get; set; }

        public string? SurName { get; set; }

        public string? LastName { get; set; }

        public DateTime? Created { get; set; }

        [Precision(3)]
        public decimal Percent { get; set; }

        [IgnoreProperty]
        public bool Set { get; set; }
    }
}
