namespace BookApi.Configuration
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
        public string BooksCollection { get; set; } = String.Empty;
        public string UsersCollection { get; set; } = String.Empty;
        public string AuthorsCollection { get; set; } = String.Empty;
        public string OrdersCollection { get; set; } = String.Empty;
    }
}