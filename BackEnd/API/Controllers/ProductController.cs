using System;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Product.Command.Models; // الخاص بـ AddProductCommand
using Core.Features.Product.Queries.Models; // الخاص بـ GetAll و GetById
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class ProductController : BaseApiController
    {
        

        [HttpPost(Router.Product.AddProduct)]
        public async Task<IActionResult> AddProduct([FromForm] AddProductCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPut(Router.Product.ChangeStatus)]
        public async Task<IActionResult> ChangeProductStatus([FromBody] ChangeProductStatusCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet(Router.Product.GetByOrgId)]
        public async Task<IActionResult> GetProductsByOrganization([FromRoute] Guid organizationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetPaginatedProductsByOrgIdQuery
            {
                OrganizationId = organizationId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        [HttpPut(Router.Product.UpdateProduct)]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete(Router.Product.DeleteProduct)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            var response = await Mediator.Send(new DeleteProductCommand(id));
            return NewResult(response);
        }

        [HttpGet(Router.Product.GetAll)] 
        public async Task<IActionResult> GetPaginatedProducts([FromQuery] GetPaginatedProductsQuery query)
        {
            var response = await Mediator.Send(query);
            return Ok(response); 
        }


        [HttpGet(Router.Product.GetById)] 
        public async Task<IActionResult> GetProductById([FromRoute] Guid id)
        {
            var response = await Mediator.Send(new GetProductByIdQuery(id));
            return NewResult(response);
        }
    }
}