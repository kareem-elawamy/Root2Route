using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Core.Features.Auctions.Commands.Models
{
    public class CancelAuctionCommand : IRequest<Response<string>>
    {
        [JsonIgnore]
        public Guid AuctionId { get; set; }
        [JsonIgnore]
        public Guid SellerId { get; set; }
    }
}
