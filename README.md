# Overhaul Entity Tracker

Dapper is great but it missed something for me and that was creating,altering and deleting tables without me having to do it manually, so i came up with this.
It uses Reflection to get type,properties etc.

Takes care of tracking the following changes.
Property added to a type -> adds a new column to the existing table
Property deleted from a type -> depending on the options it will either set the column to null if you want to keep the data or complete delete the column and all its data.
Property precision change -> change the db column type, might fail!
 
Project goals plans etc can be found here
https://gelatinous-archer-556.notion.site/Overhaul-MT-Model-Tracker-74fb2ce6fe324a9f923e8be862a36698

[![build](https://github.com/DutchJavaDev/Overhaul/actions/workflows/dotnet.yml/badge.svg)](https://github.com/DutchJavaDev/Overhaul/actions/workflows/dotnet.yml)

# Getting started
```csharp
[Table("tblDocument")]
class Document 
{
  [Key]
  public int Id { get; set; }
  [Precision(500)]
  public string Name { get; set; }
  public string Contents { get; set; }
  public DateTime DateCreated { get; set; }
}

// Define your entitty's
var types = new[] { typeof(Document) };

// Location of db
var connectionString = "";

// Create an instance
var tracker = new ModelTracker(connectionString);

// Call track methode with your entity's as parameter
tracker.Track(types);

// Get instance of the CRUD 
var crud = tracker.GetCrudInstance();

// CRUD away
var document = new Document
{
  Name = "MyDocument",
  Contents = "This is a very long string ......wq.easdasd.sad.sa.da.sd.sad.asdsa.das. I'm done typing..",
  DateCreated = DateTime.Now
};

crud.Create(document);

var _document = crud.Read<Document>();

_document.Contents = "Deleted";

crud.Update(_document);
            
crud.Delete(_document);
```
