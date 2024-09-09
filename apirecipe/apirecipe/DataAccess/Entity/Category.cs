using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apirecipe.DataAccess.Entity
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid id { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
    }
}