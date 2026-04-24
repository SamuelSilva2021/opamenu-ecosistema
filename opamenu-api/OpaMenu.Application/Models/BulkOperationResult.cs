namespace OpaMenu.Application.Models
{
    public class BulkOperationResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<int> ProcessedIds { get; set; } = new();
        public List<int> FailedIds { get; set; } = new();
    }
}
