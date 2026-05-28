using Core.Base;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Features.authentication.Commands.Models
{
    public class ChangePasswordCommand : IRequest<Response<string>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;
    }
}
