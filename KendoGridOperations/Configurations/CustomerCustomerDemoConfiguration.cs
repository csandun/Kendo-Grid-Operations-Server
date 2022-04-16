﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using KendoGridOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using KendoGridOperations.Models;

namespace KendoGridOperations.Configurations
{
    public partial class CustomerCustomerDemoConfiguration : IEntityTypeConfiguration<CustomerCustomerDemo>
    {
        public void Configure(EntityTypeBuilder<CustomerCustomerDemo> entity)
        {
            entity.HasKey(e => new { e.CustomerId, e.CustomerTypeId })
                .IsClustered(false);

            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .HasColumnName("CustomerID")
                .IsFixedLength();

            entity.Property(e => e.CustomerTypeId)
                .HasMaxLength(10)
                .HasColumnName("CustomerTypeID")
                .IsFixedLength();

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.CustomerCustomerDemo)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerCustomerDemo_Customers");

            entity.HasOne(d => d.CustomerType)
                .WithMany(p => p.CustomerCustomerDemo)
                .HasForeignKey(d => d.CustomerTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerCustomerDemo");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<CustomerCustomerDemo> entity);
    }
}