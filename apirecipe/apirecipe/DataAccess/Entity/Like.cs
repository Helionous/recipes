using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apirecipe.DataAccess.Entity
{
    public class Like
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid idRecipe { get; set; }       
        public Guid idUser { get; set; }       
        public bool status { get; set; }
    }
}