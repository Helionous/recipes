using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apirecipe.DataAccess.Generic;
using apirecipe.DataTransferObject.ObjectEnum;

namespace apirecipe.DataAccess.Entity
{
    public class Recipe : DateGeneric
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid id { get; set; }
        public string idCategory { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string instruction { get; set; }
        public string ingredient { get; set; }
        public string preparation { get; set; }
        public string cooking { get; set; }
        public string estimated { get; set; }
        public Difficulty difficulty { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
    }
}