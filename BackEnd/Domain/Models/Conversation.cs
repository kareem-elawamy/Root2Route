using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Conversation : BaseEntity
    {
        // ممكن تربط المحادثة بمحصول معين (اختياري)
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        public Guid BuyerId { get; set; } // المشتري
        public ApplicationUser Buyer { get; set; }

        public Guid SellerId { get; set; } // صاحب المحصول
        public ApplicationUser Seller { get; set; }

        public DateTime LastMessageDate { get; set; }
        public string LastMessageContent { get; set; }

        public bool IsClosed { get; set; } // عشان لو الصفقة تمت أو فشلت تقفل الشات
        public ICollection<ChatMessage> Messages { get; set; }
    }
}