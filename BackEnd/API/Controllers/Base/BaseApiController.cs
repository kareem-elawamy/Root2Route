using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Base;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shared
{
    [ApiController]
    // [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator _mediatorInstance;
        protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();

        #region Actions
        public ObjectResult NewResult<T>(Response<T> response)
        {
            // Normalize: if the handler marked the response as Succeeded but the
            // StatusCode was not explicitly set (0) or was left at BadRequest due to
            // constructor ambiguity (Response<string>("msg") { Succeeded = true }),
            // correct it to OK so the HTTP client doesn't throw.
            if (response.Succeeded && response.StatusCode != HttpStatusCode.OK
                                   && response.StatusCode != HttpStatusCode.Created
                                   && response.StatusCode != HttpStatusCode.Accepted)
            {
                response.StatusCode = HttpStatusCode.OK;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new OkObjectResult(response);
                case HttpStatusCode.Created:
                    return new CreatedResult(string.Empty, response);
                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedObjectResult(response);
                case HttpStatusCode.BadRequest:
                    return new BadRequestObjectResult(response);
                case HttpStatusCode.NotFound:
                    return new NotFoundObjectResult(response);
                case HttpStatusCode.Accepted:
                    return new AcceptedResult(string.Empty, response);
                case HttpStatusCode.UnprocessableEntity:
                    return new UnprocessableEntityObjectResult(response);
                default:
                    return new BadRequestObjectResult(response);
            }
        }
        #endregion
    }
}