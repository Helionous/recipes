using System.ComponentModel.DataAnnotations;
using apirecipe.DataTransferObject.ObjectGeneric;

namespace apirecipe.DataTransferObject.Object
{
    public class DtoVideo : DtoDateGeneric
    {
        public Guid id { get; set; }
        public string idRecipe { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "El título debe tener minimo 4 a más caracteres.")]
        public string title { get; set; }
        [Required]
        [Url(ErrorMessage = "La URL proporcionada no es válida.")]
        public string url { get; set; }
        public string description { get; set; }
    }
}
