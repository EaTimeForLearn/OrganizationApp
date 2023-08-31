using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.Collections.Generic;

namespace Persistence.Contexts
{
    public class OrganizationDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().HasKey(a => a.MemberId);
            modelBuilder.Entity<Event>().HasKey(a => a.EventId);
            modelBuilder.Entity<TicketCompany>().HasKey(a => a.CompanyId);
            modelBuilder.Entity<Ticket>().HasKey(a => a.TicketId);
            modelBuilder.Entity<EventParticipant>().HasKey(a => a.ParticipantId);
            modelBuilder.Entity<Member>().HasIndex(a => a.Email).IsUnique();
            modelBuilder.Entity<TicketCompany>().HasIndex(a => a.Mail).IsUnique();


            modelBuilder.Entity<Member>().HasData(new Member
            {
                MemberId = 1,
                Name = "Emre",
                Surname = "Akar",
                Password = "Emre1234.",
                Role = "Admin",
                Email = "emreakar@hotmail.com"

            });

            modelBuilder.Entity<Member>()
            .HasMany(e => e.Events);

            modelBuilder.Entity<Event>()
          .HasOne(e => e.Member);

            modelBuilder.Entity<Event>()
         .HasMany(e => e.Participants);

            modelBuilder.Entity<EventParticipant>()
         .HasMany(e => e.Events);

            modelBuilder.Entity<EventParticipant>()
        .HasOne(e => e.Member);

            modelBuilder.Entity<TicketCompany>()
          .HasMany(e => e.Tickets);

            modelBuilder.Entity<Ticket>()
          .HasOne(e => e.TicketCompany);

            modelBuilder.Entity<Ticket>()
           .HasOne(e => e.Event);



            //.OnDelete(DeleteBehavior.Cascade);
        }
        public OrganizationDbContext(DbContextOptions options) : base(options)
        { }
        public DbSet<Member> Members { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketCompany> TicketCompanies { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }


    }
}
