using Dapper.Contrib.Extensions;
using Overhaul.Core;

var connectionString = @"Server=86.48.0.227;Database=testapp;User Id=sa;Password=Kwende1995!;";

var tracker = new ModelTracker(connectionString);

tracker.Track(new[] { typeof(User) });

var crud = tracker.GetCrudInstance();

if (crud.Read<User>() == null)
{
    crud.Create(new User { Name = "User", Password = "Lol" });
}
else
{
    var user = crud.Read<User>();

    Console.WriteLine($"User: {user.Name}");
}

[Table("tblUser")]
class User 
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string RegisterDate { get; set; }
    public bool Active { get; set; }
}
class Login 
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

