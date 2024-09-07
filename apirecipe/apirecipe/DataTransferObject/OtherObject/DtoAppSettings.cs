namespace apirecipe.DataTransferObject.OtherObject
{
    public class DtoAppSettings
    {
        public string MongoDbConnectionString { get; set; }
        public string OriginAudience { get; set; }
        public string OriginIssuer { get; set; }
        public string AccessJwtSecret { get; set; }
        public string RefreshJwtSecret { get; set; }
        public string OriginRequest { get; set; }
    }
}
