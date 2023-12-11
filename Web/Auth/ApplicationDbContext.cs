using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Auth
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<TypeProperty> TypeProperties { get; set; }
        public  DbSet<Property> Properties { get; set; } = null;

        public DbSet<Agent> Agents { get; set; }

        public DbSet<Agency> Agencies { get; set; }

        public DbSet<PropertyImage> PropertyImages { get; set; }

        public DbSet<AgentImage>AgentImages  { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Incpections> Incpect { get; set; }


        public DbSet<ShortList> shortLists { get; set; }



        public DbSet<Comment> Comments { get; set; }

      


        public  DbSet<Feature> Features { get; set; } = null;

        public  DbSet<FeutureProperty> FeutureProperties { get; set; } = null;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>()
                 .HasMany(x => x.features)
                 .WithMany(y => y.props)
                 .UsingEntity<FeutureProperty>(
                     j => j.ToTable("FeutureProperty"));


            modelBuilder.Entity<ShortList>()
     .HasOne(sl => sl.User)
     .WithMany()
     .HasForeignKey(sl => sl.UserId)
     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShortList>()
                .HasOne(sl => sl.Property)
                .WithMany()
                .HasForeignKey(sl => sl.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);






            modelBuilder.Entity<Property>()
       .HasOne(p => p.User)
       .WithMany()
       .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Agent>()
        .HasOne(p => p.User)
        .WithMany()
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agent>()
        .HasOne(p => p.Agency)
        .WithMany()
        .HasForeignKey(p => p.AgencyId);

            modelBuilder.Entity<AgentImage>()
        .HasOne(p => p.Agent)
        .WithMany()
        .HasForeignKey(p => p.AgentId);

            modelBuilder.Entity<Agency>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId);


            modelBuilder.Entity<Comment>()
       .HasOne(p => p.User)
       .WithMany()
       .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Comment>()
       .HasOne(p => p.Agent)
       .WithMany()
       .HasForeignKey(p => p.AgentId);






            // Other entity configurations...

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany()
                .HasForeignKey(b => b.PropertyId);

            modelBuilder.Entity<Incpections>()
                .HasOne(b => b.Property)
                .WithMany()
                .HasForeignKey(b => b.PropertyId);












            // Define foreign key relationships for existing entities
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Type)
                .WithMany()
                .HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Agent)
                .WithMany()
                .HasForeignKey(p => p.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
              .HasMany(p => p.Images)
              .WithOne(pi => pi.Property)
              .HasForeignKey(pi => pi.PropertyId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
