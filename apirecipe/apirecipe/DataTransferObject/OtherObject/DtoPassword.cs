using System.ComponentModel.DataAnnotations;

namespace apirecipe.DataTransferObject.OtherObject
{
    public class DtoPassword
    {
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener minimo 6 caracteres.")]
        public string oldpassword { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener minimo 6 caracteres.")]
        public string newpassword { get; set; } 
    }
}
