using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apirecipe.DataAccess.Generic;

namespace apirecipe.DataAccess.Entity
{
    public class Video : DateGeneric
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid id { get; set; }
        public Guid idRecipe { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string? description { get; set; }
    }
}