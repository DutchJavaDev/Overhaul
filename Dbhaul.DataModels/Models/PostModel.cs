using Dbhaul.Attributes;

namespace Dbhaul.DataModels.Models
{
    [OneToMany(typeof(UserModel), nameof(UserId))]
    public sealed class PostModel
    {
        [Id]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        
    }
}
