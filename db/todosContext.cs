using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace todos.db
{
    public partial class todosContext : DbContext
    {
        public todosContext()
        {
        }

        public todosContext(DbContextOptions<todosContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Todo> Todo { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Todo>(entity =>
            {
                entity.ToTable("todo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Done).HasColumnName("done");

                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .HasColumnName("title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
