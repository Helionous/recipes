using MongoDB.Driver;
using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;

namespace apirecipe.DataAccess.Query
{
    public class QRating
    {
        public async Task<bool> IsThereRecipeRatingAsync(Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            return await dbc.Ratings.Find(u => u.idRecipe == idRecipe.ToString()).AnyAsync();
        }

        public async Task<int> CreateRatingLikedAsync(Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            DtoRating dto = new()
            {
                id = Guid.NewGuid(),
                idRecipe = idRecipe.ToString(),
                numberLike = 1,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };
            await dbc.Ratings.InsertOneAsync(AutoMapper.mapper.Map<Rating>(dto));
            return 1;
        }
        
        public async Task<int> CreateRatingDislikedAsync(Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            DtoRating dto = new()
            {
                id = Guid.NewGuid(),
                idRecipe = idRecipe.ToString(),
                numberLike = 0,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };
            await dbc.Ratings.InsertOneAsync(AutoMapper.mapper.Map<Rating>(dto));
            return 1;
        }

        public async Task<int> UpdateRatingLikedAsync(Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            UpdateDefinition<Rating> updateDefinition = Builders<Rating>.Update
                .Inc(r => r.numberLike, 1)
                .Set(r => r.updatedAt, DateTime.UtcNow);

            UpdateResult result = await dbc.Ratings.UpdateOneAsync(r => r.idRecipe == idRecipe.ToString(), updateDefinition);
            return result.ModifiedCount > 0 ? 1 : 0;
        }

        public async Task<int> UpdateRatingDislikedAsync(Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            UpdateDefinition<Rating> updateDefinition = Builders<Rating>.Update.Inc(r => r.numberLike, -1)
                .Set(r => r.updatedAt, DateTime.UtcNow);

            UpdateResult result = await dbc.Ratings.UpdateOneAsync(r => r.idRecipe == idRecipe.ToString() && r.numberLike > 0, updateDefinition);
            return result.ModifiedCount > 0 ? 1 : 0;
        }
    }
}
