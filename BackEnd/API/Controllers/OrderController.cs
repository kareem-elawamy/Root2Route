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

            // بنبعت الـ ID بتاع اليوزر عشان يجيب طلباته هو بس
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
            // بنربط الـ ID اللي جاي من الرابط بالـ Query object
            query.OrganizationId = organizationId;

            var response = await Mediator.Send(query);

            // بنستخدم Ok لأن PaginatedResult بيرجع مباشرة مش متغلف في Response العادي بتاعك
            return Ok(response);
        }
        [HttpPut(Router.Order.ChangeStatus)]
        public async Task<IActionResult> ChangeOrderStatus([FromBody] ChangeOrderStatusCommand command)
        {
            // اليوزر بيبعت الـ OrderId والـ NewStatus والـ OrganizationId بتاعته
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