using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class MonitoringContext: DbContext
{
    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Failure> Failures { get; set; }
    public DbSet<Research> Researches { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<MedicalWorker> MedicalWorkers { get; set; }
    public DbSet<ServiceHistory> ServiceHistory { get; set; }


    public MonitoringContext(DbContextOptions<MonitoringContext> options)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region DeviceType
        modelBuilder.Entity<DeviceType>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<DeviceType>()
            .Property(x=>x.UpdateTs)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<DeviceType>()
            .Property(x => x.CreateTs)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");
        #endregion

        #region Specialization
        modelBuilder.Entity<Specialization>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Specialization>()
            .Property(x => x.UpdateTs)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Specialization>()
            .Property(x => x.CreateTs)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");
        #endregion

        #region Failure
        modelBuilder.Entity<Failure>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Failure>()
            .Property(x => x.UpdateTs)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Failure>()
            .Property(x => x.CreateTs)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");
        #endregion

        #region Device
        modelBuilder.Entity<Device>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Device>()
            .Property(x => x.UpdateTs)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Device>()
            .Property(x => x.CreateTs)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Device>()
            .HasOne(x=>x.DeviceType)
            .WithMany(x=>x.Devices)
            .HasForeignKey(x=>x.DeviceTypeId);
        #endregion

        #region MedicalWorker
        modelBuilder.Entity<MedicalWorker>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<MedicalWorker>()
            .Property(x => x.UpdateTs)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<MedicalWorker>()
            .Property(x => x.CreateTs)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<MedicalWorker>()
            .HasOne(x => x.Specialization)
            .WithMany(x=>x.Workers)
            .HasForeignKey(x=>x.SpecializationId);

        #endregion

        #region Research
        modelBuilder.Entity<Research>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Research>()
            .Property(x => x.UpdateTs)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Research>()
            .Property(x => x.CreateTs)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Research>()
            .HasMany(x => x.RequiredDeviceTypes)
            .WithMany(x => x.Researches);
        #endregion

        #region ServiceHistory
        modelBuilder.Entity<ServiceHistory>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ServiceHistory>()
            .Property(x => x.UpdateTs)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<ServiceHistory>()
            .Property(x => x.CreateTs)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<ServiceHistory>()
            .HasOne(x => x.Device)
            .WithMany(x=>x.History);

        modelBuilder.Entity<ServiceHistory>()
            .HasOne(x => x.Failure);

        modelBuilder.Entity<ServiceHistory>()
            .HasOne(x => x.MedicalWorker)
            .WithMany(x=>x.History);
        #endregion
    }
}
