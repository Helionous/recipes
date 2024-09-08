using apirecipe.DataAccess.Entity;
using apirecipe.Helper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
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
                
                ApplyConventions();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public IMongoCollection<Authentication> Authentications => _database.GetCollection<Authentication>("authentications");
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Like> Likes => _database.GetCollection<Like>("likes");
        public IMongoCollection<Recipe> Recipes => _database.GetCollection<Recipe>("recipes");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("categories");
        public IMongoCollection<Rating> Ratings => _database.GetCollection<Rating>("ratings");
        public IMongoCollection<Image> Images => _database.GetCollection<Image>("images");
        public IMongoCollection<Video> Videos => _database.GetCollection<Video>("videos");
        public IMongoCollection<New> News => _database.GetCollection<New>("news");
        
        private void ApplyConventions()
        {
            var conventionPack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("EnumStringConvention", conventionPack, t => true);
        }
        
        public void Dispose()
        {
        }
    }
}