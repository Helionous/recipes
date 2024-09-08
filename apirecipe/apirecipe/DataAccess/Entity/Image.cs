using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apirecipe.DataAccess.Generic;

namespace apirecipe.DataAccess.Entity
{
    public class Image : DateGeneric
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid id { get; set; }
        public string idRecipe { get; set; }
        public string url { get; set; }
    }
}