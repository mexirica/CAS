using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservices.CAS.Business;

public class CASFile
{
    public string Name { get; set; }
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Hash { get; set; }
}
