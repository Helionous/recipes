using System.ComponentModel.DataAnnotations;
using apirecipe.DataTransferObject.ObjectEnum;
using apirecipe.DataTransferObject.ObjectGeneric;

namespace apirecipe.DataTransferObject.Object
{
    public class DtoRecipe : DtoDateGeneric
    {
        public Guid id { get; set; }

        public string idCategory { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "El título debe tener entre 10 y 255 caracteres.")]
        public string title { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 255 caracteres.")]
        public string description { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "La instrucción debe tener entre 10 y 500 caracteres.")]
        public string instruction { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 10,
            ErrorMessage = "Los ingredientes deben tener entre 10 y 255 caracteres.")]
        public string ingredient { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5,
            ErrorMessage = "El tiempo de preparación debe tener entre 5 y 50 caracteres.")]
        public string preparation { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El tiempo de cocción debe tener entre 5 y 50 caracteres.")]
        public string cooking { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El tiempo estimado debe tener entre 5 y 50 caracteres.")]
        public string estimated { get; set; }

        [Required]
        [EnumDataType(typeof(Difficulty), ErrorMessage = "La dificultad proporcionado no es válido.")]
        public Difficulty difficulty { get; set; }

        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }

        #region childs

        public ICollection<DtoImage> images { get; set; } = new List<DtoImage>();
        public ICollection<DtoVideo> videos { get; set; } = new List<DtoVideo>();
        public DtoRating rating { get; set; }

        #endregion
    }
}
