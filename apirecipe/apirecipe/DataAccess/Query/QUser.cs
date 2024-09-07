using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QUser
    {
        public void Register(DtoUser dto)
        {
            using DataBaseContext dbc = new();
            Authentication authentication = AutoMapper.mapper.Map<Authentication>(dto.authetication);
            User user = AutoMapper.mapper.Map<User>(dto);
            dbc.Authentications.InsertOne(authentication);
            dbc.Users.InsertOne(user);
        }

        public DtoUser? MyProfile(Guid id)
        {
            using DataBaseContext dbc = new();
            User user = dbc.Users.Find(u => u.id == id).FirstOrDefault();
            if (user == null) return null;
            Authentication authentication = dbc.Authentications.Find(u => u.id == Guid.Parse(user.idAuthentication)).FirstOrDefault();
            DtoUser dtoUser = AutoMapper.mapper.Map<DtoUser>(user);
            dtoUser.authetication = AutoMapper.mapper.Map<DtoAuthentication>(authentication);
            return dtoUser;
        }

        public DtoUser? GetById(Guid id)
        {
            using DataBaseContext dbc = new();
            User user = dbc.Users.Find(u => u.id == id).FirstOrDefault();
            if (user == null) return null;
            Authentication authentication = dbc.Authentications.Find(u => u.id == Guid.Parse(user.idAuthentication)).FirstOrDefault();
            DtoUser dtoUser = AutoMapper.mapper.Map<DtoUser>(user);
            dtoUser.authetication = AutoMapper.mapper.Map<DtoAuthentication>(authentication);
            return dtoUser;
        }

        public DtoUser? GetByUsername(string username)
        {
            using DataBaseContext dbc = new();
            Authentication authentication = dbc.Authentications.Find(u => u.username == username).FirstOrDefault();
            if (authentication == null) return null;
            User user = dbc.Users.Find(u => u.idAuthentication == authentication.id.ToString()).FirstOrDefault();
            DtoUser dtoUser = AutoMapper.mapper.Map<DtoUser>(user);
            dtoUser.authetication = AutoMapper.mapper.Map<DtoAuthentication>(authentication);
            return dtoUser;
        }

        public DtoUser? GetByEmail(string email)
        {
            using DataBaseContext dbc = new();
            User user = dbc.Users.Find(u => u.email == email).FirstOrDefault();
            if (user == null) return null;
            return AutoMapper.mapper.Map<DtoUser>(user);
        }
        public void Update(DtoUser dto)
        {
            using DataBaseContext dbc = new();
            User? user = AutoMapper.mapper.Map<User>(dto);
            Authentication authentication = dbc.Authentications.Find(u => u.id == Guid.Parse(user.idAuthentication)).FirstOrDefault();
            authentication.username = dto.authetication.username;
            dbc.Authentications.ReplaceOne(u => u.id == authentication.id, authentication);
            dbc.Users.ReplaceOne(u => u.id == user.id, user);
        }

        public List<DtoUser> GetAll(Guid id)
        {
            using DataBaseContext dbc = new();
            List<User> users = dbc.Users.Find(user => user.id != id).ToList();
            List<DtoUser> dtoUsers = new List<DtoUser>();
            foreach (User user in users)
            {
                DtoUser dtoUser = AutoMapper.mapper.Map<DtoUser>(user);
                Authentication authentication = dbc.Authentications.Find(u => u.id == Guid.Parse(user.idAuthentication)).FirstOrDefault();
                dtoUser.authetication = AutoMapper.mapper.Map<DtoAuthentication>(authentication);
                dtoUsers.Add(dtoUser);
            }
            return dtoUsers;
        }

        public void UpdatePassword(Guid id, string newPassword)
        {
            using DataBaseContext dbc = new();

            User user = dbc.Users.Find(u => u.id == id).FirstOrDefault();
            if (user == null) return;
            Authentication authentication = dbc.Authentications.Find(u => u.id == Guid.Parse(user.idAuthentication)).FirstOrDefault();
            user.updatedAt = DateTime.UtcNow;
            authentication.password = newPassword;
            dbc.Authentications.ReplaceOne(u => u.id == authentication.id, authentication);
            dbc.Users.ReplaceOne(u => u.id == user.id, user);
        }


        public int UpdatePromoteOrDemote(DtoUser dto)
        {
            using DataBaseContext dbc = new();
            User user = dbc.Users.Find(u => u.id == dto.id).FirstOrDefault();
            if (user == null)
                return 0;
            Authentication authentication = dbc.Authentications.Find(u => u.id == Guid.Parse(user.idAuthentication)).FirstOrDefault();
            user.updatedAt = DateTime.UtcNow;
            authentication.role = dto.authetication.role;
            authentication.status = dto.authetication.status;
            
            var authResult = dbc.Authentications.ReplaceOne(u => u.id == authentication.id, authentication);
            var userResult = dbc.Users.ReplaceOne(u => u.id == user.id, user);
            return (int)(authResult.ModifiedCount + userResult.ModifiedCount);
        }
    }
}
