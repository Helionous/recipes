using apirecipe.DataTransferObject.Object;
using apirecipe.DataTransferObject.OtherObject;
using apirecipe.Generic;

namespace apirecipe.ServiceObject
{
    public class SoAuthentication  : SoGeneric<DtoAuthentication>
    {
        public SoAuthentication()
        {
            data.additional = new Tokens();
        }
    }
}
