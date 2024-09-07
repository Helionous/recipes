namespace apirecipe.DataTransferObject.OtherObject
{
    public class DtoContainer<T>
    {
        public T dto { get; set; }
        public ICollection<T> listDto { get; set; }
        public object additional { get; set; }
    }
}