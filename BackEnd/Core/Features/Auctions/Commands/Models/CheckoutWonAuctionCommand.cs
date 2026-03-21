using MediatR;
using System;

namespace Core.Features.Auctions.Commands.Models
{
    public class CheckoutWonAuctionCommand : IRequest<Response<string>>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid AuctionId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "CashOnDelivery";
    }
}
