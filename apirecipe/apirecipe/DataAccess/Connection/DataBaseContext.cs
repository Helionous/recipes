using apirecipe.DataAccess.Entity;
using apirecipe.Helper;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Connection
{
    public class DataBaseContext : IDisposable
    {
        private readonly IMongoDatabase _database;

        public DataBaseContext()
        {
            try
            {
                string connectionString = AppSettings.GetMongoDbConnectionString();
                MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                MongoClient client = new MongoClient(settings);
                MongoUrl uri = new MongoUrl(connectionString);
                _database = client.GetDatabase(uri.DatabaseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        
        public IMongoCollection<Authentication> Authentications
        {
            get { return _database.GetCollection<Authentication>("authentications"); }
        }

        public IMongoCollection<User> Users
        {
            get { return _database.GetCollection<User>("users"); }
        }
        
        public void Dispose()
        {
            // No resources to dispose in MongoDB client
            // The method can be left empty or removed if not needed
        }
    }
}