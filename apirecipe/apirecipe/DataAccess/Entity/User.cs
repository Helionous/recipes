using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apirecipe.DataAccess.Generic;

namespace apirecipe.DataAccess.Entity
{
    public class User : DateGeneric
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid id { get; set; }
        public string idAuthentication { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        
        public Authentication ChildAthentication { get; set; }
    }
}
