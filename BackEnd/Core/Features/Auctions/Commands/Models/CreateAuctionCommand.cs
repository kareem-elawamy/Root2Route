using MediatR;
using System;

namespace Core.Features.Auctions.Commands.Models
{
    public class CreateAuctionCommand : IRequest<Response<Guid>>
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartPrice { get; set; }
        public decimal MinimumBidIncrement { get; set; }
        public decimal? ReservePrice { get; set; }
        public Guid ProductId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid SellerId { get; set; }
    }
}
