using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Product.Command.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class ProductController : BaseApiController
    {
        [HttpPost(Router.Product.cropInMarket)]
        public async Task<IActionResult> ListCropInMarket([FromBody] CreateProductFromCropCommand command)
        {
            // Mediator بيبعت الـ Command للـ Handler اللي كتبناه
            var response = await Mediator.Send(command);

            // NewResult ميثود بترجع Status Code بناءً على نجاح أو فشل الـ Response
            return NewResult(response);
        }
    }
}