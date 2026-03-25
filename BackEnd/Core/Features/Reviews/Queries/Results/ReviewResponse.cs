using System;

namespace Core.Features.Reviews.Queries.Results
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
