using apirecipe.DataTransferObject.OtherObject;

namespace apirecipe.Generic
{
    public class SoGeneric<T>
    {
        public DtoMessage message { get; set; }
        public DtoContainer<T> data { get; set; }

        public SoGeneric()
        {
            message = new DtoMessage();
            data = new DtoContainer<T>();
        }
    }
}