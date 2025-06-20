using System;
using System.Collections.Generic;
using InstaportApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MessageCentralToken> MessageCentralToken { get; set; }

    public virtual DbSet<admin> admin { get; set; }

    public virtual DbSet<cities> cities { get; set; }

    public virtual DbSet<coupons> coupons { get; set; }

    public virtual DbSet<customer_addresses> customer_addresses { get; set; }

    public virtual DbSet<customer_transactions> customer_transactions { get; set; }

    public virtual DbSet<feedbacks> feedbacks { get; set; }

    public virtual DbSet<items> items { get; set; }

    public virtual DbSet<order_addresses> order_addresses { get; set; }

    public virtual DbSet<order_past_riders> order_past_riders { get; set; }

    public virtual DbSet<order_status_updates> order_status_updates { get; set; }

    public virtual DbSet<orders> orders { get; set; }

    public virtual DbSet<price_manipulations> price_manipulations { get; set; }

    public virtual DbSet<rider_documents> rider_documents { get; set; }

    public virtual DbSet<rider_penalty> rider_penalty { get; set; }

    public virtual DbSet<rider_pointsystem> rider_pointsystem { get; set; }

    public virtual DbSet<rider_transactions> rider_transactions { get; set; }

    public virtual DbSet<riders> riders { get; set; }

    public virtual DbSet<transactions> transactions { get; set; }

    public virtual DbSet<user_penalty> user_penalty { get; set; }

    public virtual DbSet<user_pointsystem> user_pointsystem { get; set; }

    public virtual DbSet<users> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageCentralToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageC__3214EC07269614F9");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<admin>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__admin__DED88B1C185C123A");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.password).HasMaxLength(255);
            entity.Property(e => e.role).HasMaxLength(50);
            entity.Property(e => e.username).HasMaxLength(255);
        });

        modelBuilder.Entity<cities>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__cities__DED88B1C696E4955");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.city_name).HasMaxLength(255);
            entity.Property(e => e.slug).HasMaxLength(255);
        });

        modelBuilder.Entity<coupons>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__coupons__DED88B1C39EF5930");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.code).HasMaxLength(50);
            entity.Property(e => e.maxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.minimumCartValue).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<customer_addresses>(entity =>
        {
            entity.HasKey(e => e.customer_address_id).HasName("PK__customer__6105FDF2814FC71D");

            entity.Property(e => e.customer_address_id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.address).HasMaxLength(255);
            entity.Property(e => e.building_and_flat).HasMaxLength(255);
            entity.Property(e => e.floor_and_wing).HasMaxLength(255);
            entity.Property(e => e.instructions).HasMaxLength(255);
            entity.Property(e => e.key).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.phone_number).HasMaxLength(20);
            entity.Property(e => e.type).HasMaxLength(50);

            entity.HasOne(d => d.customer).WithMany(p => p.customer_addresses)
                .HasForeignKey(d => d.customer_id)
                .HasConstraintName("FK__customer___custo__1BC821DD");
        });

        modelBuilder.Entity<customer_transactions>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__customer__DED88B1CC7F9227F");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.payment_method_type).HasMaxLength(50);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.type).HasMaxLength(50);
        });

        modelBuilder.Entity<feedbacks>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__feedback__DED88B1CC3FE13BD");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.customer_name).HasMaxLength(255);
            entity.Property(e => e.feedback_type).HasMaxLength(50);
            entity.Property(e => e.rider_name).HasMaxLength(255);
        });

        modelBuilder.Entity<items>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__items__DED88B1CE983B57F");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.name).HasMaxLength(255);
        });

        modelBuilder.Entity<order_addresses>(entity =>
        {
            entity.HasKey(e => e.order_address_id).HasName("PK__order_ad__9A9DCB57E1A5000D");

            entity.Property(e => e.order_address_id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.address).HasMaxLength(255);
            entity.Property(e => e.building_and_flat).HasMaxLength(255);
            entity.Property(e => e.date).HasMaxLength(50);
            entity.Property(e => e.floor_and_wing).HasMaxLength(255);
            entity.Property(e => e.fromtime).HasMaxLength(50);
            entity.Property(e => e.key).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.phone_number).HasMaxLength(20);
            entity.Property(e => e.totime).HasMaxLength(50);
            entity.Property(e => e.type).HasMaxLength(50);

            entity.HasOne(d => d.order).WithMany(p => p.order_addresses)
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("FK__order_add__order__540C7B00");
        });

        modelBuilder.Entity<order_past_riders>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("FK__order_pas__order__57DD0BE4");
        });

        modelBuilder.Entity<order_status_updates>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.key).HasMaxLength(255);
            entity.Property(e => e.message).HasMaxLength(255);

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("FK__order_sta__order__55F4C372");
        });

        modelBuilder.Entity<orders>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__orders__DED88B1C6BE8CB09");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.commission).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.delivery_type).HasMaxLength(50);
            entity.Property(e => e.orderId).HasMaxLength(50);
            entity.Property(e => e.package).HasMaxLength(255);
            entity.Property(e => e.parcel_value).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.parcel_weight).HasMaxLength(50);
            entity.Property(e => e.payment_method).HasMaxLength(50);
            entity.Property(e => e.phone_number).HasMaxLength(20);
            entity.Property(e => e.reason).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.vehicle).HasMaxLength(50);
        });

        modelBuilder.Entity<price_manipulations>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__price_ma__DED88B1C09526B60");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.additional_drop_charge).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.additional_per_kilometer_charge).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.additional_pickup_charge).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.base_order_charges).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.cancellationCharges).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.instaport_commission).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.per_kilometer_charge).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.security_fees_charges).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.withdrawalCharges).HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<rider_documents>(entity =>
        {
            entity.HasKey(e => e.document_id).HasName("PK__rider_do__9666E8AC9BED64DA");

            entity.Property(e => e.document_id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.document_type).HasMaxLength(50);
            entity.Property(e => e.reason).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.type).HasMaxLength(50);

            entity.HasOne(d => d.rider).WithMany(p => p.rider_documents)
                .HasForeignKey(d => d.rider_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rider_doc__rider__395884C4");
        });

        modelBuilder.Entity<rider_penalty>(entity =>
        {
            entity.HasKey(e => e.riderpenaltyid);

            entity.Property(e => e.datecreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.events)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.penaltypoints).HasDefaultValue(0);
            entity.Property(e => e.riderid)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<rider_pointsystem>(entity =>
        {
            entity.HasKey(e => e.riderpointid);

            entity.Property(e => e.datecreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.events)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.points).HasDefaultValue(0);
            entity.Property(e => e.riderid)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<rider_transactions>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__rider_tr__DED88B1CE534711A");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.message).HasMaxLength(255);
            entity.Property(e => e.transactionID).HasMaxLength(255);
        });

        modelBuilder.Entity<riders>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__riders__DED88B1CDD5C9BDF");

            entity.Property(e => e._id).ValueGeneratedNever();
            entity.Property(e => e.IPID).HasMaxLength(20);
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.applied).HasDefaultValue(false);
            entity.Property(e => e.approve).HasDefaultValue(false);
            entity.Property(e => e.createdAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.fullname).HasMaxLength(255);
            entity.Property(e => e.holdAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.isDue).HasDefaultValue(false);
            entity.Property(e => e.mobileno).HasMaxLength(20);
            entity.Property(e => e.password).HasMaxLength(255);
            entity.Property(e => e.reason).HasMaxLength(255);
            entity.Property(e => e.reference_contact_1).HasMaxLength(255);
            entity.Property(e => e.requestedAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.role).HasMaxLength(50);
            entity.Property(e => e.status)
                .HasMaxLength(50)
                .HasDefaultValue("offline");
            entity.Property(e => e.updatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.wallet_amount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<transactions>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__transact__DED88B1CB2B9691D");

            entity.Property(e => e._id).ValueGeneratedNever();
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.razorpayPaymentId).HasMaxLength(100);
            entity.Property(e => e.type).HasMaxLength(50);
        });

        modelBuilder.Entity<user_penalty>(entity =>
        {
            entity.HasKey(e => e.userpenaltyid);

            entity.Property(e => e.customerid)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.datecreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.events)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.penaltypoints).HasDefaultValue(0);
        });

        modelBuilder.Entity<user_pointsystem>(entity =>
        {
            entity.HasKey(e => e.userpointid).HasName("PK__user_poi__6C1BF1A198808EFB");

            entity.Property(e => e.customerid)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.datecreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.events)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.points).HasDefaultValue(0);
        });

        modelBuilder.Entity<users>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__users__DED88B1CBC72D88F");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.__v).HasDefaultValue(0);
            entity.Property(e => e.fullname).HasMaxLength(255);
            entity.Property(e => e.holdAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.mobileno).HasMaxLength(20);
            entity.Property(e => e.password).HasMaxLength(255);
            entity.Property(e => e.role).HasMaxLength(50);
            entity.Property(e => e.usecase).HasMaxLength(50);
            entity.Property(e => e.wallet)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
