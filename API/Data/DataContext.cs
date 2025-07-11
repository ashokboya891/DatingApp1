
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Data
{
    public class DataContext:IdentityDbContext<AppUser,AppRole,int,
    IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,
    IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options):base(options)
        {

        }
        // public DbSet<AppUser>  Users{get;set;}

        public DbSet<UserLike> Likes{get;set;}
        public DbSet<Message> Messages{get;set;}
        public DbSet<Group> Groups{get;set;}
        public DbSet<Connection> connections{get;set;}

        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
            .HasMany(u => u.UserRoles)
            .WithOne(u=>u.User)
            .HasForeignKey(u=>u.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(u => u.UserRoles)
            .WithOne(u=>u.Role)
            .HasForeignKey(u=>u.RoleId)
            .IsRequired();

            builder.Entity<UserLike>().HasKey(k=>new {k.SourceUserId,k.TargetUserId});

            builder.Entity<UserLike>()
            .HasOne(s=>s.SourceUser)
            .WithMany(u=>u.LikedUsers)
            .HasForeignKey(s=>s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
            .HasOne(s=>s.TargetUser)
            .WithMany(u=>u.LikedByUsers)
            .HasForeignKey(s=>s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
            .HasOne(s=>s.Recipient)
            .WithMany(u=>u.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(s=>s.Sender)
            .WithMany(u=>u.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);

            //builder.ApplyUtcDateTimeConverter();

        }
    }
}