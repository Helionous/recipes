using apirecipe.DataAccess.Connection;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QRecipe
    {
        public bool ExistByIdCategory(Guid id)
        {
            using DataBaseContext dbc = new();
            return dbc.Recipes.Find(w => w.idCategory == id).Any();
        }
    }
}