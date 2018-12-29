using MongoDB.Driver;

namespace SaveWise.DataLayer
{
    public class SaveWiseContext : ISaveWiseContext
    {
        public IMongoDatabase Database { get; set; }

        public SaveWiseContext(string connectionString, string database)
        {           
            IMongoClient client = new MongoClient(connectionString);
            Database = client.GetDatabase(database);
        }
    }
}