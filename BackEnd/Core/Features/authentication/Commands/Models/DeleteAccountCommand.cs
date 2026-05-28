using Core.Base;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Core.Features.authentication.Commands.Models
{
    public class DeleteAccountCommand : IRequest<Response<string>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
