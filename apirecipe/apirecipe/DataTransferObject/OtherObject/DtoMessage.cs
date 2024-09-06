namespace apirecipe.DataTransferObject.OtherObject
{
    public class DtoMessage
    {
        public List<string> listMessage { get; set; }
        public string type { get; set; }

        public DtoMessage() 
        {
            listMessage = new List<string>();
        }

        public bool ExistsMessage()
        {
            return listMessage.Count > 0;
        }

        public void Success()
        {
            type = "Success";
        }

        public void Warning()
        {
            type = "Warning";
        }

        public void Error()
        {
            type = "Error";
        }   
        
        public void Exception()
        {
            type = "Exception";
        }  
    }
}