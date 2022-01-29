namespace Backend.Dtos
{
    public class SearchProductRequest
    {
        public string NameFilter { get; set; }
        public string SortBy { get; set; }
        public int? Page { get; set; }

    }
}