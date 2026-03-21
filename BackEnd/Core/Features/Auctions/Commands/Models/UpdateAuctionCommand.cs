using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Core.Features.Auctions.Commands.Models
{
    public class UpdateAuctionCommand : IRequest<Response<string>>
    {
        [JsonIgnore]
        public Guid AuctionId { get; set; }
        [JsonIgnore]
        public Guid SellerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartPrice { get; set; }
        public decimal MinimumBidIncrement { get; set; }
        public decimal? ReservePrice { get; set; }
    }
}
