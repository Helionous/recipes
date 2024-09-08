namespace apirecipe.DataTransferObject.OtherObject
{
    public class Pagination
    {
        public int pageNumber { get; set; } 
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public int totalRecords { get; set; }
    }
}
