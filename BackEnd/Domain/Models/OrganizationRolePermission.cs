using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class OrganizationRolePermission : BaseEntity
    {
        public string PermissionsClaim { get; set; } = null!;
        public Guid OrganizationRoleId { get; set; }
        public OrganizationRole? OrganizationRole { get; set; }

    }
}