using System.ComponentModel.DataAnnotations;
using apirecipe.DataTransferObject.ObjectGeneric;

namespace apirecipe.DataTransferObject.Object
{
    public class DtoImage : DtoDateGeneric
    {
        public Guid id { get; set; }
        public string idRecipe { get; set; }
        [Required]
        [Url(ErrorMessage = "La URL proporcionada no es válida.")]

        public string url { get; set; }
    }
}
