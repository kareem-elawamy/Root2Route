using System;
using System.Text.Json.Serialization;
using Core.Base;
using MediatR;

namespace Core.Features.Reviews.Commands.Models
{
    public class AddReviewCommand : IRequest<Response<Guid>>
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
        public Guid TargetOrganizationId { get; set; }
        public Guid OrderId { get; set; }
        public Guid? ProductId { get; set; }
        [System.ComponentModel.DataAnnotations.Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
