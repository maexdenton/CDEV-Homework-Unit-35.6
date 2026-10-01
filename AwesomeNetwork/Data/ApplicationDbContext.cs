using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwesomeNetwork.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Friend> Friends { get; set; } // Таблица с другами
        public DbSet<Message> Messages { get; set; } // Таблица сообщений

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Friend>().ToTable("UserFriends");
            builder.Entity<Message>().ToTable("Messages");

            // Настройка связей для сообщений
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
