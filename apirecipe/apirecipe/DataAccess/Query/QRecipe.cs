using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.OtherObject;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QRecipe
    {
        public DtoRecipe GetByTitle(string title)
        {
            using DataBaseContext dbc = new();
            string cleanedTittle = title.Replace(" ", string.Empty);
            Recipe x = dbc.Recipes.Find(w => w.title.ToLower() == cleanedTittle.ToLower()).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoRecipe>(x);
        }

        public bool ExistByTitle(string title)
        {
            using DataBaseContext dbc = new();
            string cleanedTittle = title.Replace(" ", string.Empty);
            return dbc.Recipes
                .Find(w => w.title.ToLower() == cleanedTittle.ToLower())
                .Any();
        }
        
        public bool ExistById(Guid? id)
        {
            using DataBaseContext dbc = new();
            return dbc.Recipes
                .Find(w => w.id == id)
                .Any();
        }

        public DtoRecipe GetByIdIncludeImageVideo(Guid id)
        {
            using DataBaseContext dbc = new();
            Recipe recipe = dbc.Recipes.Find(r => r.id == id).FirstOrDefault();
            if (recipe == null) return null;
            List<Image> images = dbc.Images.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
            List<Video> videos = dbc.Videos.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
            Rating rating = dbc.Ratings.Find(r => r.idRecipe == recipe.id.ToString()).FirstOrDefault();
    
            DtoRecipe dtoRecipe = AutoMapper.mapper.Map<DtoRecipe>(recipe);
            dtoRecipe.images = AutoMapper.mapper.Map<List<DtoImage>>(images);
            dtoRecipe.videos = AutoMapper.mapper.Map<List<DtoVideo>>(videos);
            dtoRecipe.rating = AutoMapper.mapper.Map<DtoRating>(rating);
    
            return dtoRecipe;
        }

        public async Task<(ICollection<DtoRecipe>, Pagination)> GetRecipesByCategory(Guid idCategory, int pageNumber, int pageSize)
        {
            using DataBaseContext dbc = new();
            long totalRecords = await dbc.Recipes.CountDocumentsAsync(r => r.idCategory == idCategory.ToString());
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            
            List<Recipe> recipes = await dbc.Recipes
                .Find(r => r.idCategory == idCategory.ToString())
                .SortBy(r => r.title)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            
            ICollection<DtoRecipe> listDtoRecipes = AutoMapper.mapper.Map<ICollection<DtoRecipe>>(recipes);
            foreach (DtoRecipe dtoRecipe in listDtoRecipes)
            {
                List<Image> images = dbc.Images.Find(r => r.idRecipe == dtoRecipe.id.ToString()).ToList();
                List<Video> videos = dbc.Videos.Find(r => r.idRecipe == dtoRecipe.id.ToString()).ToList();
                Rating rating = dbc.Ratings.Find(r => r.idRecipe == dtoRecipe.id.ToString()).FirstOrDefault();
                dtoRecipe.images = AutoMapper.mapper.Map<List<DtoImage>>(images);
                dtoRecipe.videos = AutoMapper.mapper.Map<List<DtoVideo>>(videos);
                dtoRecipe.rating = AutoMapper.mapper.Map<DtoRating>(rating);
            }

            Pagination pagination = new Pagination
            {
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalPages = totalPages,
                totalRecords = (int)totalRecords
            };
            return (listDtoRecipes, pagination);
        }
        
        public List<DtoRecipe> TopThreeMostLiked()
        {
            using DataBaseContext dbc = new();
            List<Guid> topRatedRecipeIds = dbc.Ratings
                .Find(FilterDefinition<Rating>.Empty)
                .SortByDescending(rt => rt.numberLike)
                .Limit(3)
                .ToList()
                .Select(rt => Guid.Parse(rt.idRecipe.ToString()))
                .ToList();
            
            List<Recipe> topRecipes = dbc.Recipes
                .Find(r => topRatedRecipeIds.Contains(r.id))
                .ToList()
                .OrderBy(r => topRatedRecipeIds.IndexOf(r.id))
                .ToList();
            
            List<DtoRecipe> listDtoRecipes = AutoMapper.mapper.Map<List<DtoRecipe>>(topRecipes);
            foreach (DtoRecipe dtoRecipe in listDtoRecipes)
            {
                List<Image> images = dbc.Images.Find(r => r.idRecipe == dtoRecipe.id.ToString()).ToList();
                List<Video> videos = dbc.Videos.Find(r => r.idRecipe == dtoRecipe.id.ToString()).ToList();
                Rating rating = dbc.Ratings.Find(r => r.idRecipe == dtoRecipe.id.ToString()).FirstOrDefault();
                dtoRecipe.images = AutoMapper.mapper.Map<List<DtoImage>>(images);
                dtoRecipe.videos = AutoMapper.mapper.Map<List<DtoVideo>>(videos);
                dtoRecipe.rating = AutoMapper.mapper.Map<DtoRating>(rating);
            }
            return listDtoRecipes;
        }

        public List<DtoRecipe> GetAll()
        {
            using DataBaseContext dbc = new();
            List<Recipe> recipes = dbc.Recipes.Find(FilterDefinition<Recipe>.Empty).SortBy(ob => ob.updatedAt).ToList();
            List<DtoRecipe> dtoRecipe = AutoMapper.mapper.Map<List<DtoRecipe>>(recipes);
            foreach (DtoRecipe recipe in dtoRecipe)
            {
                List<Image> images = dbc.Images.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
                List<Video> videos = dbc.Videos.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
                Rating rating = dbc.Ratings.Find(r => r.idRecipe == recipe.id.ToString()).FirstOrDefault();
                recipe.images = AutoMapper.mapper.Map<List<DtoImage>>(images);
                recipe.videos = AutoMapper.mapper.Map<List<DtoVideo>>(videos);
                recipe.rating = AutoMapper.mapper.Map<DtoRating>(rating);
            }
            return dtoRecipe;
        }

        public int CreateRecipe(DtoRecipe dtoRecipe, Guid idUser)
        {
            using DataBaseContext dbc = new();
            dtoRecipe.id = Guid.NewGuid();
            dtoRecipe.createdAt = DateTime.UtcNow;
            dtoRecipe.updatedAt = DateTime.UtcNow;
            dtoRecipe.createdBy = idUser.ToString();
            dtoRecipe.updatedBy = idUser.ToString();

            foreach (DtoImage dtoImage in dtoRecipe.images)
            {
                dtoImage.id = Guid.NewGuid();
                dtoImage.idRecipe = dtoRecipe.id.ToString();
                dtoImage.createdAt = DateTime.UtcNow;
                dtoImage.updatedAt = DateTime.UtcNow;
                
                Image image = AutoMapper.mapper.Map<Image>(dtoImage);
                dbc.Images.InsertOne(image);
            }

            if (dtoRecipe.videos?.Count > 0)
            {
                foreach (DtoVideo dtoVideo in dtoRecipe.videos)
                {
                    dtoVideo.id = Guid.NewGuid();
                    dtoVideo.idRecipe = dtoRecipe.id.ToString();
                    dtoVideo.createdAt = DateTime.UtcNow;
                    dtoVideo.updatedAt = DateTime.UtcNow;
                    Video video = AutoMapper.mapper.Map<Video>(dtoVideo);
                    dbc.Videos.InsertOne(video);
                }
            }

            Recipe recipe = AutoMapper.mapper.Map<Recipe>(dtoRecipe);
            dbc.Recipes.InsertOne(recipe);
            return 1;
        }

        public int UpdateRecipe(DtoRecipe dtoRecipe, Guid idUser)
        {
            using DataBaseContext dbc = new();
            Recipe recipe = dbc.Recipes.Find(r => r.id == dtoRecipe.id).FirstOrDefault();
            
            if (recipe != null)
            {
                recipe.title = dtoRecipe.title;
                recipe.description = dtoRecipe.description;
                recipe.ingredient = dtoRecipe.ingredient;
                recipe.preparation = dtoRecipe.preparation;
                recipe.cooking = dtoRecipe.cooking;
                recipe.estimated = dtoRecipe.estimated;
                recipe.difficulty = dtoRecipe.difficulty;
                recipe.updatedAt = DateTime.UtcNow;
                recipe.updatedBy = idUser.ToString();

                foreach (DtoImage dtoImage in dtoRecipe.images)
                {
                    Image? existingImage = dbc.Images.Find(r => r.id == dtoImage.id).FirstOrDefault();
                    if (existingImage != null)
                    {
                        existingImage.url = dtoImage.url;
                        existingImage.updatedAt = DateTime.UtcNow;
                        dbc.Images.ReplaceOne(r => r.id == dtoImage.id, existingImage);
                    }
                }

                foreach (DtoVideo dtoVideo in dtoRecipe.videos)
                {
                    Video? existingVideo = dbc.Videos.Find(r => r.id == dtoVideo.id).FirstOrDefault();
                    if (existingVideo != null)
                    {
                        existingVideo.title = dtoVideo.title;
                        existingVideo.url = dtoVideo.url;
                        existingVideo.description = dtoVideo.description;
                        existingVideo.updatedAt = DateTime.UtcNow;
                        dbc.Videos.ReplaceOne(r => r.id == dtoVideo.id, existingVideo);
                    }
                }

                dbc.Recipes.ReplaceOne(r => r.id == dtoRecipe.id, recipe);
                return 1;
            }
            return 0;
        }

        public int DeleteOnCascade(Guid id)
        {
            using DataBaseContext dbc = new();
            Recipe recipe = dbc.Recipes.Find(r => r.id == id).FirstOrDefault();
            if (recipe != null)
            {
                List<Image> images = dbc.Images.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
                if (images.Count > 0)
                {
                    dbc.Images.DeleteMany(i => i.idRecipe == recipe.id.ToString());
                }
                List<Video> videos = dbc.Videos.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
                if (videos.Count > 0)
                {
                    dbc.Videos.DeleteMany(v => v.idRecipe == recipe.id.ToString());
                }
                Rating rating = dbc.Ratings.Find(r => r.idRecipe == recipe.id.ToString()).FirstOrDefault();
                if (rating != null)
                {
                    dbc.Ratings.DeleteOne(r => r.idRecipe == rating.id.ToString());
                }
                
                List<Like> likes = dbc.Likes.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
                if (likes.Count > 0)
                { 
                    dbc.Likes.DeleteMany(l => l.idUser == recipe.id.ToString());
                }
                dbc.Recipes.DeleteOne(r => r.id == id);
                return 1;
            }
            return 0;
        }

        public bool ExistByIdCategory(Guid id)
        {
            using DataBaseContext dbc = new();
            return dbc.Recipes.Find(w => w.idCategory == id.ToString()).Any();
        }

        public List<DtoRecipe> RecipesYouLiked(Guid id)
        {
            using DataBaseContext dbc = new();
            List<Like> likes = dbc.Likes.Find(l => l.idUser == id.ToString() && l.status).ToList();
            List<DtoRecipe> dtoRecipes = new();
            
            foreach (Like like in likes)
            {
                Recipe recipe = dbc.Recipes.Find(r => r.id == Guid.Parse(like.idRecipe)).FirstOrDefault();
                List<Image> images = dbc.Images.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
                List<Video> videos = dbc.Videos.Find(r => r.idRecipe == recipe.id.ToString()).ToList();
                Rating rating = dbc.Ratings.Find(r => r.idRecipe == recipe.id.ToString()).FirstOrDefault();
    
                DtoRecipe dtoRecipe = AutoMapper.mapper.Map<DtoRecipe>(recipe);
                dtoRecipe.images = AutoMapper.mapper.Map<List<DtoImage>>(images);
                dtoRecipe.videos = AutoMapper.mapper.Map<List<DtoVideo>>(videos);
                dtoRecipe.rating = AutoMapper.mapper.Map<DtoRating>(rating);
                dtoRecipes.Add(dtoRecipe);
            }
            return dtoRecipes;
        }
    }
}
