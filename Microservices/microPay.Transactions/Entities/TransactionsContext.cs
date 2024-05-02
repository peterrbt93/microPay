using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace microPay.Transactions.Entities;

public partial class TransactionsContext : DbContext
{
    public TransactionsContext()
    {
    }

    public TransactionsContext(DbContextOptions<TransactionsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transactions");

            entity.Property(e => e.AccountId).HasMaxLength(45);
            entity.Property(e => e.Action).HasMaxLength(45);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
