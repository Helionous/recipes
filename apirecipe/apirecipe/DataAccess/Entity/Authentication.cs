using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apirecipe.DataTransferObject.ObjectEnum;

namespace apirecipe.DataAccess.Entity
{
    public class Authentication
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public Role role { get; set; }
        public bool status { get; set; }
        
        public User ParentUser { get; set; }
    }
}
