using apirecipe.DataAccess.Generic;

namespace apirecipe.DataAccess.Entity
{
    public class Rating : DateGeneric
    {
        public Guid id { get; set; }
        public string idRecipe { get; set; }
        public long numberLike { get; set; }
    }
}