using MongoDB.Driver;

namespace SaveWise.DataLayer
{
    public class SaveWiseContext
    {
        public IMongoDatabase Database { get; set; }

        public SaveWiseContext(string connectionString, string database)
        {
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(database);
        }
    }
}