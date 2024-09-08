using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apirecipe.DataAccess.Generic;

namespace apirecipe.DataAccess.Entity
{
    public class New : DateGeneric
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid id { get; set; }   
        public Guid? idRecipe { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string content { get; set; }
        public bool status { get; set; }
        public string url { get; set; }
        public DateTime deletedAt { get; set; }
    }
}