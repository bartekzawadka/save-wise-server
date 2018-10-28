using MongoDB.Driver;

namespace SaveWise.DataLayer
{
    public interface ISaveWiseContext
    {
        IMongoDatabase Database { get; set; }
    }
}