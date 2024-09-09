using MongoDB.Driver;
using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;

namespace apirecipe.DataAccess.Query
{
    public class QLike
    {
        public async Task<int> GiveLikeAsync(DtoLike dto)
        {
            using DataBaseContext dbc = new();
            Like like = AutoMapper.mapper.Map<Like>(dto);
            like.id = Guid.NewGuid();
            await dbc.Likes.InsertOneAsync(like);
            return 1;
        }

        public async Task<bool> HasUserLikedRecipeAsync(Guid idUser, Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            return await dbc.Likes.Find(like => like.idUser == idUser.ToString() && like.idRecipe == idRecipe.ToString()).AnyAsync();
        }

        public async Task<DtoLike> GetByIdsAsync(Guid idUser, Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            Like? like = await dbc.Likes.Find(like => like.idUser == idUser.ToString() && like.idRecipe == idRecipe.ToString()).FirstOrDefaultAsync();
            return AutoMapper.mapper.Map<DtoLike>(like);
        }

        public async Task<int> UpdateStatusAsync(Guid idUser, Guid idRecipe, bool status)
        {
            using DataBaseContext dbc = new();
            UpdateDefinition<Like> updateDefinition = Builders<Like>.Update.Set(l => l.status, status);
            UpdateResult result = await dbc.Likes.UpdateOneAsync(
                like => like.idUser == idUser.ToString() && like.idRecipe == idRecipe.ToString(),
                updateDefinition
            );
            return result.ModifiedCount > 0 ? 1 : 0;
        }
    }
}