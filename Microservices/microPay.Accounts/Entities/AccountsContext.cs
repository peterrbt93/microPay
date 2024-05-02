using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace microPay.Accounts.Entities;

public partial class AccountsContext : DbContext
{
    public AccountsContext()
    {
    }

    public AccountsContext(DbContextOptions<AccountsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("accounts");

            entity.Property(e => e.CanOverdraft).HasColumnName("canOverdraft");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(45);
            entity.Property(e => e.Username).HasMaxLength(45);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
