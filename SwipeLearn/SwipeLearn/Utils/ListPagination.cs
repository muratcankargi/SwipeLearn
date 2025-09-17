namespace SwipeLearn.Utils
{
    public class ListPagination<T>
    {
        public PagingInfo pageInfo { get; set; }
        public List<T> list { get; set; }
    }
    public class PagingInfo
    {
        public int page { get; set; } = 1;
        public int size { get; set; } = 50;
        public int total { get; set; }
        public int max { get; set; }
    }
}
