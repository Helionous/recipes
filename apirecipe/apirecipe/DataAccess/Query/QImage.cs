using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QImage
    {
        public DtoImage GetImageByUrl(string url)
        {
            using DataBaseContext dbc = new();
            Image image = dbc.Images.Find(i => i.url == url).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoImage>(image);
        }
        
        public DtoImage GetImageByIdRecipe(Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            Image image = dbc.Images.Find(i => i.idRecipe == idRecipe.ToString()).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoImage>(image);
        }

        public List<DtoImage> GetAll()
        {
            using DataBaseContext dbc = new();
            List<Image> images = dbc.Images.Find(_ => true).SortBy(i => i.updatedAt).ToList();
            return AutoMapper.mapper.Map<List<DtoImage>>(images);
        }
        
        public bool ExistImageById(Guid id)
        {
            using DataBaseContext dbc = new();
            return dbc.Images.Find(i => i.id == id).Any();
        }
    }
}