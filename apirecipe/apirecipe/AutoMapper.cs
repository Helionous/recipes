using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using AutoMapper;

namespace apirecipe
{
    public class AutoMapper
    {
        private static bool _initMapper = true;
        public static IMapper mapper;
        
        public static void Start()
        {
            if (_initMapper)
            {
                MapperConfiguration configuration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Authentication, DtoAuthentication>().ReverseMap();
                    cfg.CreateMap<User, DtoUser>().ReverseMap();
                    cfg.CreateMap<Like, DtoLike>().ReverseMap();
                    cfg.CreateMap<Category, DtoCategory>().ReverseMap();
                    cfg.CreateMap<Recipe, DtoRecipe>().ReverseMap();
                    cfg.CreateMap<Rating, DtoRating>().ReverseMap();
                    cfg.CreateMap<Image, DtoImage>().ReverseMap();
                    cfg.CreateMap<Video, DtoVideo>().ReverseMap();
                    cfg.CreateMap<New, DtoNew>().ReverseMap();
                });

                mapper = configuration.CreateMapper();
                _initMapper = false;
            }
        }   
    }
}
