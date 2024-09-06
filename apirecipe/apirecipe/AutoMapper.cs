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

                    cfg.CreateMap<User, DtoUser>()
                        .ForMember(dest => dest.authetication, opt => opt.MapFrom(src => src.ChildAthentication))
                        .ReverseMap();

                    cfg.CreateMap<DtoUser, User>()
                        .ForMember(dest => dest.ChildAthentication, opt => opt.MapFrom(src => src.authetication));
                });

                mapper = configuration.CreateMapper();
                _initMapper = false;
            }
        }   
    }
}
