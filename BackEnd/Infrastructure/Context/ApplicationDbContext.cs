using Domain.Models; // تأكد من الـ Namespace
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // =========================================================
        // 1. SaaS & Organizations Module
        // =========================================================
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationMember> OrganizationMembers { get; set; }
        public DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<OrganizationRolePermission> OrganizationRolePermissions { get; set; }

        // =========================================================
        // 2. Market & Products Module
        // =========================================================
        public DbSet<MarketItem> MarketItems { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CropActivityLog> CropActivityLogs { get; set; }

        // =========================================================
        // 3. Commerce & Auctions Module
        // =========================================================
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }

        // =========================================================
        // 4. Knowledge Base & Reviews
        // =========================================================
        public DbSet<PlantInfo> PlantInfos { get; set; }
        public DbSet<PlantGuideStep> PlantGuideSteps { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. Security Schema ---
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "security");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles", "security");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "security");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "security");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "security");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "security");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "security");

            // --- 2. Inheritance (TPH) ---
            modelBuilder.Entity<MarketItem>()
                .ToTable("MarketItems")
                .HasDiscriminator<string>("ItemType")
                .HasValue<Product>("Product");

            // منع حذف المؤسسة إذا كان لديها منتجات
            modelBuilder.Entity<MarketItem>()
                .HasOne(m => m.Organization)
                .WithMany()
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- 3. Dynamic Roles Relationships ---

            // OrganizationMember -> OrganizationRole
            modelBuilder.Entity<OrganizationMember>()
                .HasOne(m => m.OrganizationRole)
                .WithMany()
                .HasForeignKey(m => m.OrganizationRoleId)
                .OnDelete(DeleteBehavior.SetNull); // لو الرول اتمسحت، الموظف يفضل موجود

            // OrganizationRole -> Permissions
            modelBuilder.Entity<OrganizationRole>()
                .HasMany(r => r.Permissions)
                .WithOne(p => p.OrganizationRole) // تأكد أن الاسم في الكلاس OrganizationRole
                .HasForeignKey(p => p.OrganizationRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MarketItem>()
                    .HasOne(m => m.Organization)          // MarketItem عنده Organization واحدة
                    .WithMany()                           // (مؤقتاً) Organization عنده items كتير (لو مش ضايف List هناك سيبها فاضية)
                    .HasForeignKey(m => m.OrganizationId) // ده الأهم: إجبار EF على استخدام OrganizationId اللي أنت عملته
                    .OnDelete(DeleteBehavior.Restrict);
            // --- 4. Decimal Precision ---
            var decimalProps = new[]
            {
                (typeof(Auction), "StartPrice"),
                (typeof(Auction), "CurrentHighestBid"),
                (typeof(Bid), "Amount"),
                (typeof(Order), "TotalAmount"),
                (typeof(OrderItem), "UnitPrice"),
            };

            foreach (var prop in decimalProps)
            {
                modelBuilder.Entity(prop.Item1).Property(prop.Item2).HasColumnType("decimal(18,2)");
            }

            // --- 5. Fixes for OrderItem Relationship ---

            // ✅ التصحيح هنا: HasOne تأخذ الـ Navigation Property (الكائن) وليس الـ Id
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MarketItem)  // الكائن
                .WithMany()
                .HasForeignKey(oi => oi.MarketItemId) // الـ ID
                .OnDelete(DeleteBehavior.Restrict);


            // --- 6. Other Relationships ---
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.TargetUser)
                .WithMany()
                .HasForeignKey(r => r.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany() // تأكد من وجود ICollection<Bid> في ApplicationUser أو اتركها فارغة
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Sender)
                .WithMany()
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Receiver)
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrganizationMember>()
                .HasOne(om => om.Organization)
                .WithMany(om => om.Members)
                .HasForeignKey(om => om.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrganizationMember>()
                .HasOne(om => om.User)
                .WithMany()
                .HasForeignKey(om => om.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}