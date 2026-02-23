using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ChatMessage : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public Guid SenderId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; }

        // لإضافة طابع "مستقل":
        public bool IsSystemMessage { get; set; } // هل دي رسالة تلقائية من السيستم؟
        public string? AttachmentUrl { get; set; } // لو هيبعت صورة المحصول أو العقد
    }


}
