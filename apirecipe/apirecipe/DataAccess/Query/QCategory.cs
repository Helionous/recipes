using apirecipe.DataAccess.Connection;
using apirecipe.DataAccess.Entity;
using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.OtherObject;
using MongoDB.Driver;

namespace apirecipe.DataAccess.Query
{
    public class QCategory
    {   
        public async Task<(ICollection<DtoCategory>, Pagination)> GetWithOffsetPagination(int pageNumber, int pageSize)
        {
            using DataBaseContext dbc = new();
            int totalRecords = (int)await dbc.Categories.CountDocumentsAsync(Builders<Category>.Filter.Empty);
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            var categories = await dbc.Categories
                .Find(Builders<Category>.Filter.Empty)
                .SortBy(p => p.name)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            ICollection<DtoCategory> listDtoCategories = AutoMapper.mapper.Map<ICollection<DtoCategory>>(categories);
            Pagination pagination = new Pagination
            {
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalPages = totalPages,
                totalRecords = totalRecords
            };
            return (listDtoCategories, pagination);
        }
        
        public List<DtoCategory> GetAll()
        {
            using DataBaseContext dbc = new();
            var categories = dbc.Categories.Find(Builders<Category>.Filter.Empty)
                .SortBy(ob => ob.name)
                .ToList();
            return AutoMapper.mapper.Map<List<DtoCategory>>(categories);
        }
        
        public int Create(DtoCategory dto)
        {
            using DataBaseContext dbc = new();
            dto.id = Guid.NewGuid();
            var category = AutoMapper.mapper.Map<Category>(dto);
            dbc.Categories.InsertOne(category);
            return 1;
        }
        
        public int Update(DtoCategory dto)
        {
            using DataBaseContext dbc = new();
            var category = AutoMapper.mapper.Map<Category>(dto);
            var result = dbc.Categories.ReplaceOne(c => c.id == category.id, category);
            return (int)result.ModifiedCount;
        }
        
        public DtoCategory? GetByName(string name)
        {
            using DataBaseContext dbc = new();
            string cleanedName = name.Replace(" ", string.Empty);
            Category? category = dbc.Categories
                .Find(w => w.name == cleanedName)
                .FirstOrDefault();
            return AutoMapper.mapper.Map<DtoCategory>(category);
        }
        
        public DtoCategory GetById(Guid id)
        {
            using DataBaseContext dbc = new();
            var category = dbc.Categories.Find(u => u.id == id).FirstOrDefault();
            return AutoMapper.mapper.Map<DtoCategory>(category);
        }
        
        public bool ExistCategoryById(Guid id)
        {
            using DataBaseContext dbc = new();
            return dbc.Categories.Find(w => w.id == id).Any();
        }
        
        public int Delete(Guid id)
        {
            using DataBaseContext dbc = new();
            var result = dbc.Categories.DeleteOne(i => i.id == id);
            return (int)result.DeletedCount;
        }
    }
}
