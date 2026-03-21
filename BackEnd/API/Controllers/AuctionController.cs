using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Auctions.Commands.Models;
using Core.Features.Auctions.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class AuctionController : BaseApiController
    {
        [HttpPost(Router.Auction.CreateAuction)]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            command.SellerId = userId.Value;

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPost(Router.Auction.PlaceBid)]
        public async Task<IActionResult> PlaceBid([FromRoute] Guid auctionId, [FromBody] PlaceBidCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            
            command.AuctionId = auctionId;
            command.BidderId = userId.Value;

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [AllowAnonymous]
        [HttpGet(Router.Auction.GetActive)]
        public async Task<IActionResult> GetActiveAuctions([FromQuery] GetActiveAuctionsQuery query)
        {
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [AllowAnonymous]
        [HttpGet(Router.Auction.GetById)]
        public async Task<IActionResult> GetAuctionById([FromRoute] Guid id)
        {
            var response = await Mediator.Send(new GetAuctionByIdQuery(id));
            return NewResult(response);
        }

        [AllowAnonymous]
        [HttpGet(Router.Auction.GetCompleted)]
        public async Task<IActionResult> GetCompletedAuctions([FromQuery] GetCompletedAuctionsQuery query)
        {
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [AllowAnonymous]
        [HttpGet(Router.Auction.Prefix + "/{auctionId}/bids")]
        public async Task<IActionResult> GetAuctionBids([FromRoute] Guid auctionId)
        {
            var response = await Mediator.Send(new GetAuctionBidsQuery(auctionId));
            return NewResult(response);
        }

        [HttpPut(Router.Auction.UpdateAuction)]
        public async Task<IActionResult> UpdateAuction([FromRoute] Guid auctionId, [FromBody] UpdateAuctionCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            command.AuctionId = auctionId;
            command.SellerId = userId.Value;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete(Router.Auction.CancelAuction)]
        public async Task<IActionResult> CancelAuction([FromRoute] Guid auctionId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var command = new CancelAuctionCommand { AuctionId = auctionId, SellerId = userId.Value };
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet(Router.Auction.GetMyOrgAuctions)]
        public async Task<IActionResult> GetMyOrganizationAuctions([FromRoute] Guid organizationId)
        {
            var response = await Mediator.Send(new GetMyOrganizationAuctionsQuery(organizationId));
            return NewResult(response);
        }

        [HttpGet(Router.Auction.GetMyWonAuctions)]
        public async Task<IActionResult> GetMyWonAuctions()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var response = await Mediator.Send(new GetMyWonAuctionsQuery(userId.Value));
            return NewResult(response);
        }

        [HttpGet(Router.Auction.GetMyParticipated)]
        public async Task<IActionResult> GetMyParticipatedAuctions()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var response = await Mediator.Send(new GetMyParticipatedAuctionsQuery(userId.Value));
            return NewResult(response);
        }

        [HttpPost(Router.Auction.Checkout)]
        public async Task<IActionResult> Checkout([FromRoute] Guid id, [FromBody] CheckoutWonAuctionCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            command.AuctionId = id;
            command.UserId = userId.Value;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return null;
            return Guid.Parse(userIdString);
        }
    }
}
