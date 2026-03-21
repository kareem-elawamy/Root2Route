using MediatR;
using System;

namespace Core.Features.Auctions.Commands.Models
{
    public class CheckoutWonAuctionCommand : IRequest<Response<Guid>>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid AuctionId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "CashOnDelivery";
    }
}
