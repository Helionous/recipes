namespace apirecipe.Helper
{
    public class NewsDeletionService
    {
        
        //private readonly QNew _qNew;

        public NewsDeletionService()
        {
            //_qNew = new QNew();
        }
        /*
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    List<DtoNew> allNews = _qNew.NewsWithExpiredDates();
                    if (allNews.Count != 0)
                    {
                        foreach (DtoNew news in allNews)
                        {
                            if (news.deletedAt < DateTime.UtcNow && news.deletedAt.Hour < DateTime.UtcNow.Hour && news.deletedAt.Minute <= DateTime.UtcNow.Minute)
                            {
                                _qNew.Delete(news.id);
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Error de servicio de eliminación de noticias");
            return base.StopAsync(cancellationToken);
        }*/
    }
}
