using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QNew
    {
        public int CreateNew(DtoNew dto)
        {
            using DataBaseContext dbc = new();
            dto.id = Guid.NewGuid();
            dto.status = true;
            dto.createdAt = DateTime.UtcNow;
            dto.updatedAt = DateTime.UtcNow;
            New news = AutoMapper.mapper.Map<New>(dto);
            dbc.News.InsertOne(news);
            return 1;
        }
        
        public int UpdateNew(DtoNew dto)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.id, dto.id);
            var update = Builders<New>.Update
                .Set(n => n.idRecipe, dto.idRecipe)
                .Set(n => n.title, dto.title)
                .Set(n => n.subtitle, dto.subtitle)
                .Set(n => n.content, dto.content)
                .Set(n => n.status, dto.status)
                .Set(n => n.url, dto.url)
                .Set(n => n.deletedAt, dto.deletedAt)
                .Set(n => n.updatedAt, DateTime.UtcNow);
            var result = dbc.News.UpdateOne(filter, update);
            return (int)result.ModifiedCount;
        }
        
        public DtoNew GetById(Guid id)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.id, id);
            New x = dbc.News.Find(filter).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoNew>(x);
        }
        
        public List<DtoNew> GetAll()
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Empty;
            return AutoMapper.mapper.Map<List<DtoNew>>(dbc.News.Find(filter)
                .SortBy(n => n.updatedAt)
                .ToList());
        }
        
        public int Delete(Guid id)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.id, id);
            var result = dbc.News.DeleteOne(filter);
            return (int)result.DeletedCount;
        }
        
        public bool ExistById(Guid? id)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.id, id);
            return dbc.News.Find(filter).Any();
        }
        
        public bool ExistByTitle(string title)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.title, title);
            return dbc.News.Find(filter).Any();
        }
        
        public bool ExistByUrl(string url)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.url, url);
            return dbc.News.Find(filter).Any();
        }
        
        public DtoNew GetByTitle(string title)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.title, title);
            New x = dbc.News.Find(filter).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoNew>(x);
        }
        
        public DtoNew GetByUrl(string url)
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Eq(n => n.url, url);
            New x = dbc.News.Find(filter).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoNew>(x);
        }
        
        public List<DtoNew> NewsWithExpiredDates()
        {
            using DataBaseContext dbc = new();
            var filter = Builders<New>.Filter.Where(n => n.deletedAt < DateTime.UtcNow &&
                                                         n.deletedAt.Hour < DateTime.UtcNow.Hour &&
                                                         n.deletedAt.Minute <= DateTime.UtcNow.Minute);
            return AutoMapper.mapper.Map<List<DtoNew>>(dbc.News.Find(filter)
                .SortBy(n => n.updatedAt)
                .ToList());
        }
    }
}
