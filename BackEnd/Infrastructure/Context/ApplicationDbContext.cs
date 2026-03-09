using Domain.Common;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }

        // =========================================================
        // 2. Market & Products Module
        // =========================================================
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; } // تم التعديل لحرف P كابيتال

        // =========================================================
        // 3. Commerce & Auctions Module
        // =========================================================
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }

        // =========================================================
        // 4. Knowledge Base & Communication
        // =========================================================
        public DbSet<PlantInfo> PlantInfos { get; set; }
        public DbSet<PlantGuideStep> PlantGuideSteps { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> Chats { get; set; }
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

            // --- 2. Relationships & Constraints ---

            // 🛑 (تم حذف كود الوراثة الخاص بـ MarketItem لأنه لم يعد موجوداً)

            // Product -> Organization (تم تعديله من MarketItem إلى Product)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Organization)
                .WithMany() // لو Organization مفيهاش ICollection<Product>
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductImage -> Product (علاقة جديدة)
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // لو المنتج اتمسح، صوره تتمسح معاه

            // OrderItem -> Product (تم تعديله من MarketItem)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.product)
                .WithMany()
                .HasForeignKey(oi => oi.productid)
                .OnDelete(DeleteBehavior.Restrict);

            // Auction -> Product (تأكد من تعديل الـ FK في كيان الـ Auction ليكون ProductId)
            modelBuilder.Entity<Auction>()
                .HasOne(a => a.product)
                .WithMany()
                .HasForeignKey(a => a.productid)
                .OnDelete(DeleteBehavior.Restrict);

            // OrganizationMember -> OrganizationRole
            modelBuilder.Entity<OrganizationMember>()
                .HasMany(m => m.OrganizationRoles)
                .WithMany();

            // OrganizationRole -> Permissions
            modelBuilder.Entity<OrganizationRole>()
                .HasMany(r => r.Permissions)
                .WithOne(p => p.OrganizationRole)
                .HasForeignKey(p => p.OrganizationRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review Relationships
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

            // Bid Relationships
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrganizationMember Relationships
            modelBuilder.Entity<OrganizationMember>()
                .HasOne(om => om.Organization)
                .WithMany(o => o.Members)
                .HasForeignKey(om => om.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationMember>()
                .HasOne(om => om.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(om => om.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Conversation
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Buyer)
                .WithMany()
                .HasForeignKey(c => c.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Seller)
                .WithMany()
                .HasForeignKey(c => c.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatMessage
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- 3. Decimal Precision ---
            var decimalProps = new[]
            {
                (typeof(Auction), "StartPrice"),
                (typeof(Auction), "CurrentHighestBid"),
                (typeof(Bid), "Amount"),
                (typeof(Order), "TotalAmount"),
                (typeof(OrderItem), "UnitPrice"),
                (typeof(Product), "DirectSalePrice"),   // تم إضافة أسعار المنتج
                (typeof(Product), "StartBiddingPrice")
            };

            foreach (var prop in decimalProps)
            {
                modelBuilder.Entity(prop.Item1).Property(prop.Item2).HasColumnType("decimal(18,2)");
            }

            // =============================================================
            // 🔥 تطبيق Soft Delete تلقائياً (نسخة مصححة لمشاكل الوراثة)
            // =============================================================
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && entityType.BaseType == null)
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "x");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var notDeleted = Expression.Not(property);
                    var lambda = Expression.Lambda(notDeleted, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        // =============================================================
        // 🔥 تحويل الحذف إلى Soft Delete عند الحفظ
        // =============================================================
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; // حوله لتعديل
                        entry.Entity.IsDeleted = true;      // علم عليه كمحذوف
                        entry.Entity.UpdatedAt = DateTime.UtcNow; // سجل وقت الحذف
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}