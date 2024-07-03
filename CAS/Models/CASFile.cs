using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservices.CAS.Models;
public class CASFile
{
    public string? Name { get; set; }
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Hash { get; set; }
}
