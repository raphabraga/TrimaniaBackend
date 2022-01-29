namespace Backend.Dtos
{
    public class SearchUserRequest
    {
        public string Filter { get; set; }
        public string SortBy { get; set; }
        public int? Page { get; set; }

    }
}