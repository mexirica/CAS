using Microservices.CAS.Business;
using MongoDB.Driver;

namespace Microservices.CAS.Db;

public class CASFileRepository(MongoDBContext context) : IRepository<CASFile>
{
    public async Task<IEnumerable<CASFile>> GetAllAsync()
    {
        return await context.CASFiles.Find(_ => true).ToListAsync();
    }

    public async Task<CASFile> GetByIdAsync(string id)
    {
        return await context.CASFiles.Find(CASFile => CASFile.Hash == id).FirstOrDefaultAsync();
    }
    
    public async Task<CASFile> GetByNameAsync(string name)
    {
        return await context.CASFiles.Find(CASFile => CASFile.Name == name).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(CASFile CASFile)
    {
        await context.CASFiles.InsertOneAsync(CASFile);
    }

    public async Task UpdateAsync(string id, CASFile CASFileIn)
    {
        await context.CASFiles.ReplaceOneAsync(CASFile => CASFile.Hash == id, CASFileIn);
    }

    public async Task DeleteAsync(string id)
    {
        await context.CASFiles.DeleteOneAsync(CASFile => CASFile.Hash == id);
    }
}
