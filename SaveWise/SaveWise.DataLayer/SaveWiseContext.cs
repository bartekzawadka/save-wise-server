using MongoDB.Driver;

namespace SaveWise.DataLayer
{
    public class SaveWiseContext : ISaveWiseContext
    {
        private IMongoClient _client;
        
        public IMongoDatabase Database { get; set; }

        public SaveWiseContext(string connectionString, string database)
        {
            _client = new MongoClient(connectionString);
            Database = _client.GetDatabase(database);
        }
    }
}