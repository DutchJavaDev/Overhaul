using Dbhaul.Attributes;

namespace Dbhaul.DataModels.Models
{
    [ManyToMany(typeof(TagModel))]
    public sealed class ArticleModel
    {
        [Id]
        public Guid Id { get; set; }

        public string? Title { get; set; }
    }
}
