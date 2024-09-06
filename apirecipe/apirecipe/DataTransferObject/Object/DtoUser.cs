using System.ComponentModel.DataAnnotations;
using apirecipe.DataTransferObject.ObjectGeneric;

namespace apirecipe.DataTransferObject.Object
{
    public class DtoUser : DtoDateGeneric
    {
        public Guid id { get; set; }
        public string idAuthentication { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string firstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El apellido debe tener entre 3 y 100 caracteres.")]
        public string lastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico es incorrecto.")]
        [StringLength(50, ErrorMessage = "El correo electrónico no puede exceder los 50 caracteres.")]
        public string email { get; set; }

        public DtoAuthentication authetication { get; set; } = null!;
    }
}