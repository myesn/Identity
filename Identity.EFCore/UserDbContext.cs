using Microsoft.EntityFrameworkCore;

namespace Upo.Identity.EntityFrameworkCore
{
    public class IdentityDbContext : DbContext, IUserStore
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
            this.Context = this;
        }
        public DbSet<IdentityOrganization> Organizations { get; set; }
        public DbSet<IdentityUser> Users { get; set; }
        public DbSet<IdentityOrganizationUser> OrganizationUsers { get; set; }
        public DbSet<IdentityToken> Tokens { get; set; }
        public DbContext Context { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityOrganization>(entity =>
            {
                entity.ToTable("Organizations");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.CreateTime).HasColumnType("datetime");

                entity
                    .HasOne(x => x.Parent)
                    .WithMany(x => x.Children)
                    .HasForeignKey(x => x.ParentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Gender).HasConversion<string>();
                entity.Property(x => x.IsEnabled).HasConversion<string>();
                entity.Property(x => x.CreateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<IdentityOrganizationUser>(entity =>
            {
                entity.ToTable("OrganizationUsers");
                entity.HasKey(x => new { x.OrganizationId, x.UserId });

                entity.Property(x => x.IsPrimary).HasConversion<string>();

                entity
                    .HasOne(x => x.Organization)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity
                   .HasOne(x => x.User)
                   .WithMany(x => x.Organizations)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<IdentityToken>(entity =>
            {
                entity.ToTable("Tokens");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.IssueTime).HasColumnType("datetime");
                entity.Property(x => x.ExpiredTime).HasColumnType("datetime");

                entity
                    .HasOne(x => x.User)
                    .WithMany(x => x.Tokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });
        }
    }
}