using Dbhaul.Attributes;

namespace Dbhaul.DataModels.Models
{
    public sealed class TagModel
    {
        [Id]
        public Guid Id { get; set; }

        public string? Title { get; set; }
    }
}
