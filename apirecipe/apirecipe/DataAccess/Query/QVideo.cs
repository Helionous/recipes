using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QVideo
    {
        public DtoVideo GetVideoByUrl(string url)
        {
            using DataBaseContext dbc = new();
            Video video = dbc.Videos.Find(v => v.url == url).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoVideo>(video);
        }
        
        public DtoVideo GetVideoByIdRecipe(Guid idRecipe)
        {
            using DataBaseContext dbc = new();
            Video video = dbc.Videos.Find(v => v.idRecipe == idRecipe.ToString()).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoVideo>(video);
        }
        
        public bool ExistVideoById(Guid id)
        {
            using DataBaseContext dbc = new();
            return dbc.Videos.Find(v => v.id == id).Any();
        }
    }
}