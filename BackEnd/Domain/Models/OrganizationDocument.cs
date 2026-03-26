using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models
{
    public class OrganizationDocument : BaseEntity
    {
        public Guid OrganizationId { get; set; }
        
        [ForeignKey(nameof(OrganizationId))]
        public Organization? Organization { get; set; }

        public OrganizationDocumentType Type { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
