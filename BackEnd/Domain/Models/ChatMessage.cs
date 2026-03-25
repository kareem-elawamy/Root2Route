using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class ChatMessage : BaseEntity
    {
        public Guid ChatRoomId { get; set; }
        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom? ChatRoom { get; set; }

        public Guid SenderId { get; set; }
        [ForeignKey(nameof(SenderId))]
        public ApplicationUser? Sender { get; set; }

        public string Content { get; set; } = string.Empty;
        
        public MessageType Type { get; set; } = MessageType.Text;
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Negotiation Offer Specifics
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ProposedPrice { get; set; }
        public int? ProposedQuantity { get; set; }
        public Guid? RelatedOrderId { get; set; }
    }
}
