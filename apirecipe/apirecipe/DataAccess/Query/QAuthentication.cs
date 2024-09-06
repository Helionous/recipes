using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QAuthentication
    {
        public DtoAuthentication GetByUsername(string username)
        {
            using DataBaseContext dbc = new();
            Authentication authentication = dbc.Authentications.Find(u => u.username == username).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoAuthentication>(authentication);
        }

        public bool ExistByUsername(string username)
        {
            using DataBaseContext dbc = new();
            return dbc.Authentications.Find(u => u.username == username).Any();
        }

        public DtoUser GetUserByIdUsername(string username)
        {
            using DataBaseContext dbc = new();
            Authentication authentication = dbc.Authentications.Find(a => a.username == username).FirstOrDefault();
            User user = dbc.Users.Find(u => u.idAuthentication == authentication.id.ToString()).FirstOrDefault();
            
            DtoUser dtoUser = AutoMapper.mapper.Map<DtoUser>(user);
            dtoUser.authetication = AutoMapper.mapper.Map<DtoAuthentication>(authentication);
            return dtoUser;
        }
    }
}