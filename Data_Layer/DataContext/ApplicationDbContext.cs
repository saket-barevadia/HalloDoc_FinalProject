using System;
using System.Collections.Generic;
using Data_Layer.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.DataContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Adminregion> Adminregions { get; set; }

    public virtual DbSet<Aspnetrole> Aspnetroles { get; set; }

    public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

    public virtual DbSet<Aspnetuserrole> Aspnetuserroles { get; set; }

    public virtual DbSet<Blockrequest> Blockrequests { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<Casetag> Casetags { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Concierge> Concierges { get; set; }

    public virtual DbSet<Emaillog> Emaillogs { get; set; }

    public virtual DbSet<EncounterForm> EncounterForms { get; set; }

    public virtual DbSet<Healthprofessional> Healthprofessionals { get; set; }

    public virtual DbSet<Healthprofessionaltype> Healthprofessionaltypes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<PayRate> PayRates { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<Physicianlocation> Physicianlocations { get; set; }

    public virtual DbSet<Physiciannotification> Physiciannotifications { get; set; }

    public virtual DbSet<Physicianregion> Physicianregions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Requestbusiness> Requestbusinesses { get; set; }

    public virtual DbSet<Requestclient> Requestclients { get; set; }

    public virtual DbSet<Requestclosed> Requestcloseds { get; set; }

    public virtual DbSet<Requestconcierge> Requestconcierges { get; set; }

    public virtual DbSet<Requestnote> Requestnotes { get; set; }

    public virtual DbSet<Requeststatuslog> Requeststatuslogs { get; set; }

    public virtual DbSet<Requesttype> Requesttypes { get; set; }

    public virtual DbSet<Requestwisefile> Requestwisefiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rolemenu> Rolemenus { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Shiftdetail> Shiftdetails { get; set; }

    public virtual DbSet<Shiftdetailregion> Shiftdetailregions { get; set; }

    public virtual DbSet<Smslog> Smslogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WeeklyTimeSheet> WeeklyTimeSheets { get; set; }

    public virtual DbSet<WeeklyTimeSheetDetail> WeeklyTimeSheetDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=S@ket6898;Server=localhost;Port=5432;Database=HalloDocDb;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Adminid).HasName("admin_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.AdminAspnetusers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("admin_aspnetuserid_fkey");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.AdminCreatedbyNavigations).HasConstraintName("admin_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.AdminModifiedbyNavigations).HasConstraintName("admin_modifiedby_fkey");
        });

        modelBuilder.Entity<Adminregion>(entity =>
        {
            entity.HasKey(e => e.Adminregionid).HasName("adminregion_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.Adminregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adminregion_adminid_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Adminregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adminregion_regionid_fkey");
        });

        modelBuilder.Entity<Aspnetrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aspnetroles_pkey");
        });

        modelBuilder.Entity<Aspnetuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aspnetusers_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");
        });

        modelBuilder.Entity<Aspnetuserrole>(entity =>
        {
            entity.HasKey(e => e.Aspnetuserrolesid).HasName("aspnetuserroles_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Aspnetuserroles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aspnetuserroles_roleid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserroles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aspnetuserroles_userid_fkey");
        });

        modelBuilder.Entity<Blockrequest>(entity =>
        {
            entity.HasKey(e => e.Blockrequestid).HasName("blockrequests_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Blockrequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("blockrequests_requestid_fkey");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Businessid).HasName("business_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.BusinessCreatedbyNavigations).HasConstraintName("business_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.BusinessModifiedbyNavigations).HasConstraintName("business_modifiedby_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Businesses).HasConstraintName("business_regionid_fkey");
        });

        modelBuilder.Entity<Casetag>(entity =>
        {
            entity.HasKey(e => e.Casetagid).HasName("casetag_pkey");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("Chat_pkey");
        });

        modelBuilder.Entity<Concierge>(entity =>
        {
            entity.HasKey(e => e.Conciergeid).HasName("concierge_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.Region).WithMany(p => p.Concierges).HasConstraintName("concierge_regionid_fkey");
        });

        modelBuilder.Entity<Emaillog>(entity =>
        {
            entity.HasKey(e => e.Emaillogid).HasName("emaillog_pkey");

            entity.Property(e => e.Createdate).HasDefaultValueSql("CURRENT_DATE");
        });

        modelBuilder.Entity<EncounterForm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EncounterForm_pkey");

            entity.Property(e => e.IsFinalized).HasDefaultValueSql("'0'::\"bit\"");

            entity.HasOne(d => d.Request).WithMany(p => p.EncounterForms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_encounter_request");
        });

        modelBuilder.Entity<Healthprofessional>(entity =>
        {
            entity.HasKey(e => e.Vendorid).HasName("healthprofessionals_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.ProfessionNavigation).WithMany(p => p.Healthprofessionals).HasConstraintName("healthprofessionals_profession_fkey");
        });

        modelBuilder.Entity<Healthprofessionaltype>(entity =>
        {
            entity.HasKey(e => e.Healthprofessionalid).HasName("healthprofessionaltype_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Menuid).HasName("menu_pkey");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderdetails_pkey");
        });

        modelBuilder.Entity<PayRate>(entity =>
        {
            entity.HasKey(e => e.PayRateId).HasName("PayRate_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.PayRates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("phyrate_physicianid_fk");
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.Physicianid).HasName("physician_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.PhysicianAspnetusers).HasConstraintName("physician_aspnetuserid_fkey");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.PhysicianCreatedbyNavigations).HasConstraintName("physician_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.PhysicianModifiedbyNavigations).HasConstraintName("physician_modifiedby_fkey");
        });

        modelBuilder.Entity<Physicianlocation>(entity =>
        {
            entity.HasKey(e => e.Locationid).HasName("physicianlocation_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Physicianlocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physicianlocation_physicianid_fkey");
        });

        modelBuilder.Entity<Physiciannotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("physiciannotification_pkey");

            entity.Property(e => e.Isnotificationstopped).HasDefaultValueSql("'0'::\"bit\"");

            entity.HasOne(d => d.Physician).WithMany(p => p.Physiciannotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physiciannotification_physicianid_fkey");
        });

        modelBuilder.Entity<Physicianregion>(entity =>
        {
            entity.HasKey(e => e.Physicianregionid).HasName("physicianregion_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Physicianregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physicianregion_physicianid_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Physicianregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("physicianregion_regionid_fkey");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Regionid).HasName("region_pkey");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Requestid).HasName("request_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.Requesttypeid).HasDefaultValueSql("1");
            entity.Property(e => e.Status).HasDefaultValueSql("1");

            entity.HasOne(d => d.Physician).WithMany(p => p.Requests).HasConstraintName("request_physicianid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Requests).HasConstraintName("request_userid_fkey");
        });

        modelBuilder.Entity<Requestbusiness>(entity =>
        {
            entity.HasKey(e => e.Requestbusinessid).HasName("requestbusiness_pkey");

            entity.HasOne(d => d.Business).WithMany(p => p.Requestbusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestbusiness_businessid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestbusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestbusiness_requestid_fkey");
        });

        modelBuilder.Entity<Requestclient>(entity =>
        {
            entity.HasKey(e => e.Requestclientid).HasName("requestclient_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Requestclients).HasConstraintName("requestclient_regionid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestclients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestclient_requestid_fkey");
        });

        modelBuilder.Entity<Requestclosed>(entity =>
        {
            entity.HasKey(e => e.Requestclosedid).HasName("requestclosed_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestcloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestclosed_requestid_fkey");

            entity.HasOne(d => d.Requeststatuslog).WithMany(p => p.Requestcloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestclosed_requeststatuslogid_fkey");
        });

        modelBuilder.Entity<Requestconcierge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("requestconcierge_pkey");

            entity.HasOne(d => d.Concierge).WithMany(p => p.Requestconcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestconcierge_conciergeid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestconcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestconcierge_requestid_fkey");
        });

        modelBuilder.Entity<Requestnote>(entity =>
        {
            entity.HasKey(e => e.Requestnotesid).HasName("requestnotes_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.RequestnoteCreatedbyNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestnotes_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.RequestnoteModifiedbyNavigations).HasConstraintName("requestnotes_modifiedby_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestnotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestnotes_requestid_fkey");
        });

        modelBuilder.Entity<Requeststatuslog>(entity =>
        {
            entity.HasKey(e => e.Requeststatuslogid).HasName("requeststatuslog_pkey");

            entity.Property(e => e.Status).HasDefaultValueSql("1");

            entity.HasOne(d => d.Admin).WithMany(p => p.Requeststatuslogs).HasConstraintName("requeststatuslog_adminid_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.RequeststatuslogPhysicians).HasConstraintName("requeststatuslog_physicianid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requeststatuslogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requeststatuslog_requestid_fkey");

            entity.HasOne(d => d.Transtophysician).WithMany(p => p.RequeststatuslogTranstophysicians).HasConstraintName("requeststatuslog_transtophysicianid_fkey");
        });

        modelBuilder.Entity<Requesttype>(entity =>
        {
            entity.HasKey(e => e.Requesttypeid).HasName("requesttype_pkey");
        });

        modelBuilder.Entity<Requestwisefile>(entity =>
        {
            entity.HasKey(e => e.Requestwisefileid).HasName("requestwisefile_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.Admin).WithMany(p => p.Requestwisefiles).HasConstraintName("requestwisefile_adminid_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Requestwisefiles).HasConstraintName("requestwisefile_physicianid_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Requestwisefiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requestwisefile_requestid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("role_pkey");

            entity.Property(e => e.Createdby).HasDefaultValueSql("1");
            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");
        });

        modelBuilder.Entity<Rolemenu>(entity =>
        {
            entity.HasKey(e => e.Rolemenuid).HasName("rolemenu_pkey");

            entity.HasOne(d => d.Menu).WithMany(p => p.Rolemenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rolemenu_menuid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Rolemenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rolemenu_roleid_fkey");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Shiftid).HasName("shift_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.Isrepeat).HasDefaultValueSql("'0'::\"bit\"");
            entity.Property(e => e.Startdate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.Weekdays).IsFixedLength();

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shift_createdby_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shift_physicianid_fkey");
        });

        modelBuilder.Entity<Shiftdetail>(entity =>
        {
            entity.HasKey(e => e.Shiftdetailid).HasName("shiftdetail_pkey");

            entity.Property(e => e.Isdeleted).HasDefaultValueSql("'0'::\"bit\"");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.Shiftdetails).HasConstraintName("shiftdetail_modifiedby_fkey");

            entity.HasOne(d => d.Shift).WithMany(p => p.Shiftdetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shiftdetail_shiftid_fkey");
        });

        modelBuilder.Entity<Shiftdetailregion>(entity =>
        {
            entity.HasKey(e => e.Shiftdetailregionid).HasName("shiftdetailregion_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Shiftdetailregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shiftdetailregion_regionid_fkey");

            entity.HasOne(d => d.Shiftdetail).WithMany(p => p.Shiftdetailregions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shiftdetailregion_shiftdetailid_fkey");
        });

        modelBuilder.Entity<Smslog>(entity =>
        {
            entity.HasKey(e => e.Smslogid).HasName("smslog_pkey");

            entity.Property(e => e.Createdate).HasDefaultValueSql("CURRENT_DATE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.Property(e => e.Createddate).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.UserAspnetusers).HasConstraintName("users_aspnetuserid_fkey");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.UserCreatedbyNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.UserModifiedbyNavigations).HasConstraintName("users_modifiedby_fkey");
        });

        modelBuilder.Entity<WeeklyTimeSheet>(entity =>
        {
            entity.HasKey(e => e.TimeSheetId).HasName("WeeklyTimeSheet_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.WeeklyTimeSheets).HasConstraintName("WeeklyTimeSheet_AdminId_fkey");

            entity.HasOne(d => d.PayRate).WithMany(p => p.WeeklyTimeSheets).HasConstraintName("WeeklyTimeSheet_PayRateId_fkey");

            entity.HasOne(d => d.Provider).WithMany(p => p.WeeklyTimeSheets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("WeeklyTimeSheet_ProviderId_fkey");
        });

        modelBuilder.Entity<WeeklyTimeSheetDetail>(entity =>
        {
            entity.HasKey(e => e.TimeSheetDetailId).HasName("WeeklyTimeSheetDetail_pkey");

            entity.HasOne(d => d.TimeSheet).WithMany(p => p.WeeklyTimeSheetDetails).HasConstraintName("WeeklyTimeSheetDetail_TimeSheetId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
