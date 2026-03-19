using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Orders.Commands.Models;
using Core.Features.Orders.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize] 
    public class OrderController : BaseApiController
    {
       
        [HttpPost(Router.Order.CreateOrder)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            command.BuyerId = userId.Value;

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

    
        [HttpGet(Router.Order.GetMyOrders)]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var response = await Mediator.Send(new GetMyOrdersQuery(userId.Value));
            return NewResult(response);
        }

        
        [HttpGet(Router.Order.GetOrderById)]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var response = await Mediator.Send(new GetOrderByIdQuery(id, userId.Value));
            return NewResult(response);
        }
        [HttpGet(Router.Order.Prefix + "/Received/{organizationId}")]
        public async Task<IActionResult> GetReceivedOrders([FromRoute] Guid organizationId, [FromQuery] GetReceivedOrdersQuery query)
        {
            query.OrganizationId = organizationId;

            var response = await Mediator.Send(query);

            return Ok(response);
        }
       [HttpPut(Router.Order.ChangeStatus)]
public async Task<IActionResult> ChangeOrderStatus([FromBody] ChangeOrderStatusCommand command)
{
    // 2. Safely extract OrganizationId from Users JWT instead of body
    var orgIdString = User.FindFirst("OrganizationId")?.Value; // Match this with your exact Claim Type
    if (string.IsNullOrEmpty(orgIdString) || !Guid.TryParse(orgIdString, out Guid orgId))
    {
        return Unauthorized("User does not belong to a valid organization.");
    }

    command.OrganizationId = orgId;

    var response = await Mediator.Send(command);
    return NewResult(response);
}


        [HttpPut(Router.Order.CancelOrder)]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var command = new CancelOrderCommand
            {
                OrderId = id,
                BuyerId = userId.Value
            };

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        #region Helpers

        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return null;

            return Guid.Parse(userIdString);
        }

        #endregion
    }
}