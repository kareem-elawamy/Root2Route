using MediatR;
using System;

namespace Core.Features.Auctions.Commands.Models
{
    public class PlaceBidCommand : IRequest<Response<string>>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid AuctionId { get; set; }
        public decimal Amount { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Guid BidderId { get; set; }
    }
}
