namespace Microservices.CAS.Db.Repository.Interfaces
{
	public interface ICacheRepository 
	{
		public ValueTask<string> GetByKeyAsync(string key);
		public ValueTask SetByKeyAsync(string key, string value);
		public ValueTask RemoveByKeyAsync(string key);

	}
}
