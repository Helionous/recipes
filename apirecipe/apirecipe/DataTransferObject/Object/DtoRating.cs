using apirecipe.DataTransferObject.ObjectGeneric;

namespace apirecipe.DataTransferObject.Object
{
    public class DtoRating : DtoDateGeneric
    {
        public Guid id { get; set; }
        public string idRecipe { get; set; }
        public long numberLike { get; set; }
    }
}
