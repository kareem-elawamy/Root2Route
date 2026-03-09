using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Base;
using MediatR;

namespace Core.Features.OrganizationRole.Commands.Models
{
    public class AddOrganizationRoleCommand : IRequest<Response<string>>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsSystemDefault { get; set; } = false;
        public List<string> Permissions { get; set; } = new List<string>();
        [JsonIgnore] // هذا الحقل داخلي فقط، لا يأتي من الفرونت إند
        public Guid OrganizationId { get; set; }
        [JsonIgnore] // هذا الحقل داخلي فقط، لا يأتي من الفرونت إند
        public Guid RequesterUserId { get; set; }
    }
}