namespace NodeTrees.BusinessLogic.Models
{
    public sealed class Range<T>
    {
        public int Skip { get; set; }
        public int Count { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
