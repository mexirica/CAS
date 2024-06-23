using Microservices.CAS.Db.Repository.Interfaces;
using StackExchange.Redis;

namespace Microservices.CAS.Db.Repository
{
	public class RedisRepository(IDatabase redis) : ICacheRepository
	{
		public async ValueTask<string> GetByKeyAsync(string key)
		{
			return await redis.StringGetAsync(key);
		}

		public async ValueTask SetByKeyAsync(string key, string value)
		{
			await redis.StringSetAsync(key, value);
		}

		public async ValueTask RemoveByKeyAsync(string key)
		{
			await redis.KeyDeleteAsync(key);
		}

	}
}
