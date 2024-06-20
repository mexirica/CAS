using Microservices.CAS.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Microservices.CAS.Db;

public class MongoDBContext 
{
    private readonly IMongoDatabase _database;

    public MongoDBContext(IOptions<MongoDBSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<CASFile> CASFiles => _database.GetCollection<CASFile>("CASFile");
}

public class MongoDBSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}
