using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public class EmployeeManagerDbContext(DbContextOptions<EmployeeManagerDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    public DbSet<Workplace> Workplaces => Set<Workplace>();

    public DbSet<WorkplaceMember> WorkplaceMembers => Set<WorkplaceMember>();

    public DbSet<Invitation> Invitations => Set<Invitation>();

    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();

    public DbSet<Shift> Shifts => Set<Shift>();

    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureAppUser(modelBuilder.Entity<AppUser>());
        ConfigureWorkplace(modelBuilder.Entity<Workplace>());
        ConfigureWorkplaceMember(modelBuilder.Entity<WorkplaceMember>());
        ConfigureInvitation(modelBuilder.Entity<Invitation>());
        ConfigureAvailabilitySlot(modelBuilder.Entity<AvailabilitySlot>());
        ConfigureShift(modelBuilder.Entity<Shift>());
        ConfigureWorkLog(modelBuilder.Entity<WorkLog>());
    }

    private static void ConfigureAppUser(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AppUser> entity)
    {
        entity.ToTable("app_users");

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.ExternalAuthUserId).HasColumnName("external_auth_user_id").HasMaxLength(200).IsRequired();
        entity.Property(x => x.AuthProvider).HasColumnName("auth_provider").HasMaxLength(50).IsRequired();
        entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        entity.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(200);
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

        entity.HasIndex(x => x.ExternalAuthUserId).IsUnique();
    }

    private static void ConfigureWorkplace(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Workplace> entity)
    {
        entity.ToTable("workplaces");

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        entity.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

        entity.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedWorkplaces)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_workplaces_created_by_user");
    }

    private static void ConfigureWorkplaceMember(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<WorkplaceMember> entity)
    {
        entity.ToTable("workplace_members", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_workplace_members_role", "role IN ('Manager', 'Employee')");
        });

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.WorkplaceId).HasColumnName("workplace_id");
        entity.Property(x => x.UserId).HasColumnName("user_id");
        entity.Property(x => x.Role).HasColumnName("role").HasMaxLength(30).HasConversion<string>().IsRequired();
        entity.Property(x => x.JoinedAt).HasColumnName("joined_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

        entity.HasIndex(x => x.UserId).HasDatabaseName("ix_workplace_members_user_id");
        entity.HasIndex(x => x.WorkplaceId).HasDatabaseName("ix_workplace_members_workplace_id");
        entity.HasIndex(x => new { x.WorkplaceId, x.UserId }).IsUnique().HasDatabaseName("uq_workplace_members_workplace_user");

        entity.HasOne(x => x.Workplace)
            .WithMany(x => x.WorkplaceMembers)
            .HasForeignKey(x => x.WorkplaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_workplace_members_workplace");

        entity.HasOne(x => x.User)
            .WithMany(x => x.WorkplaceMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_workplace_members_user");
    }

    private static void ConfigureInvitation(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Invitation> entity)
    {
        entity.ToTable("invitations", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_invitations_status", "status IN ('Pending', 'Accepted', 'Expired', 'Cancelled')");
            tableBuilder.HasCheckConstraint("ck_invitations_invited_role", "invited_role IN ('Manager', 'Employee')");
        });

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.WorkplaceId).HasColumnName("workplace_id");
        entity.Property(x => x.InvitedEmail).HasColumnName("invited_email").HasMaxLength(255).IsRequired();
        entity.Property(x => x.InvitedRole).HasColumnName("invited_role").HasMaxLength(30).HasConversion<string>().HasDefaultValue(WorkplaceRole.Employee).IsRequired();
        entity.Property(x => x.Token).HasColumnName("token").HasMaxLength(300).IsRequired();
        entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).HasConversion<string>().HasDefaultValue(InvitationStatus.Pending).IsRequired();
        entity.Property(x => x.InvitedByMemberId).HasColumnName("invited_by_member_id");
        entity.Property(x => x.AcceptedByMemberId).HasColumnName("accepted_by_member_id");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
        entity.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamp with time zone");
        entity.Property(x => x.AcceptedAt).HasColumnName("accepted_at").HasColumnType("timestamp with time zone");

        entity.HasIndex(x => x.WorkplaceId).HasDatabaseName("ix_invitations_workplace_id");
        entity.HasIndex(x => x.InvitedEmail).HasDatabaseName("ix_invitations_invited_email");
        entity.HasIndex(x => x.Token).IsUnique();

        entity.HasOne(x => x.Workplace)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.WorkplaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_invitations_workplace");

        entity.HasOne(x => x.InvitedByMember)
            .WithMany(x => x.InvitationsSent)
            .HasForeignKey(x => x.InvitedByMemberId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_invitations_invited_by_member");

        entity.HasOne(x => x.AcceptedByMember)
            .WithMany(x => x.AcceptedInvitations)
            .HasForeignKey(x => x.AcceptedByMemberId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_invitations_accepted_by_member");
    }

    private static void ConfigureAvailabilitySlot(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AvailabilitySlot> entity)
    {
        entity.ToTable("availability_slots", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_availability_slots_time", "start_time < end_time");
        });

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.WorkplaceMemberId).HasColumnName("workplace_member_id");
        entity.Property(x => x.Date).HasColumnName("date");
        entity.Property(x => x.StartTime).HasColumnName("start_time");
        entity.Property(x => x.EndTime).HasColumnName("end_time");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

        entity.HasIndex(x => new { x.WorkplaceMemberId, x.Date }).HasDatabaseName("ix_availability_slots_member_date");

        entity.HasOne(x => x.WorkplaceMember)
            .WithMany(x => x.AvailabilitySlots)
            .HasForeignKey(x => x.WorkplaceMemberId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_availability_slots_workplace_member");
    }

    private static void ConfigureShift(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Shift> entity)
    {
        entity.ToTable("shifts", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_shifts_time", "start_time < end_time");
        });

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.WorkplaceId).HasColumnName("workplace_id");
        entity.Property(x => x.WorkplaceMemberId).HasColumnName("workplace_member_id");
        entity.Property(x => x.Date).HasColumnName("date");
        entity.Property(x => x.StartTime).HasColumnName("start_time");
        entity.Property(x => x.EndTime).HasColumnName("end_time");
        entity.Property(x => x.AssignedByMemberId).HasColumnName("assigned_by_member_id");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

        entity.HasIndex(x => new { x.WorkplaceId, x.Date }).HasDatabaseName("ix_shifts_workplace_date");
        entity.HasIndex(x => new { x.WorkplaceMemberId, x.Date }).HasDatabaseName("ix_shifts_member_date");

        entity.HasOne(x => x.Workplace)
            .WithMany(x => x.Shifts)
            .HasForeignKey(x => x.WorkplaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_shifts_workplace");

        entity.HasOne(x => x.WorkplaceMember)
            .WithMany(x => x.ReceivedShifts)
            .HasForeignKey(x => x.WorkplaceMemberId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_shifts_workplace_member");

        entity.HasOne(x => x.AssignedByMember)
            .WithMany(x => x.AssignedShifts)
            .HasForeignKey(x => x.AssignedByMemberId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_shifts_assigned_by_member");
    }

    private static void ConfigureWorkLog(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<WorkLog> entity)
    {
        entity.ToTable("work_logs", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_work_logs_actual_time", "actual_start_time IS NULL OR actual_end_time IS NULL OR actual_start_time < actual_end_time");
            tableBuilder.HasCheckConstraint("ck_work_logs_status", "status IN ('Completed', 'PartiallyWorked', 'Absent', 'Cancelled')");
        });

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.ShiftId).HasColumnName("shift_id");
        entity.Property(x => x.ActualStartTime).HasColumnName("actual_start_time");
        entity.Property(x => x.ActualEndTime).HasColumnName("actual_end_time");
        entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasDefaultValue(WorkLogStatus.Completed).IsRequired();
        entity.Property(x => x.Note).HasColumnName("note");
        entity.Property(x => x.RecordedByMemberId).HasColumnName("recorded_by_member_id");
        entity.Property(x => x.RecordedAt).HasColumnName("recorded_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

        entity.HasIndex(x => x.ShiftId).IsUnique();

        entity.HasOne(x => x.Shift)
            .WithOne(x => x.WorkLog)
            .HasForeignKey<WorkLog>(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_work_logs_shift");

        entity.HasOne(x => x.RecordedByMember)
            .WithMany(x => x.RecordedWorkLogs)
            .HasForeignKey(x => x.RecordedByMemberId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_work_logs_recorded_by_member");
    }
}
