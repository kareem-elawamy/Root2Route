using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Farm : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
    
        // المزرعة ملك "مؤسسة" (حتى لو فردية)
        public Guid OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public ICollection<Crop> Crops { get; set; } = new List<Crop>();
    }
}
