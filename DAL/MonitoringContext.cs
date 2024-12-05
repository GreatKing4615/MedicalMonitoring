using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class MonitoringContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Research> Researches { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<ServiceHistory> ServiceHistories { get; set; }
    public DbSet<ResearchHistory> ResearchHistories { get; set; }
    public DbSet<SimulationResult> SimulationResults { get; set; }
    public DbSet<EquipmentLoadForecast> EquipmentLoadForecasts { get; set; }



    public MonitoringContext(DbContextOptions<MonitoringContext> options)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
        #endregion

        #region User
        modelBuilder.Entity<User>()
            .HasKey(x => x.Id);
        #endregion

        #region Research
        modelBuilder.Entity<Research>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<Research>()
            .Property(r => r.DeviceTypes)
            .HasColumnType("integer[]");

        #endregion

        #region ResearchHistory
        modelBuilder.Entity<ResearchHistory>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ResearchHistory>()
            .HasOne(x => x.Research)
            .WithMany(x => x.History);

        modelBuilder.Entity<ResearchHistory>()
            .HasOne(x => x.Device)
            .WithMany(x => x.ResearchHistory);

        modelBuilder.Entity<ResearchHistory>()
            .Property(x => x.StartTime)
            .IsRequired();

        modelBuilder.Entity<ResearchHistory>()
            .Property(x => x.EndTime)
            .IsRequired();
        #endregion

        #region ServiceHistory
        modelBuilder.Entity<ServiceHistory>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ServiceHistory>()
            .HasOne(x => x.Responsible)
            .WithMany(x=>x.History);

        modelBuilder.Entity<ServiceHistory>()
            .HasOne(x => x.Device)
            .WithMany(x=>x.ServiceHistory);

        modelBuilder.Entity<ServiceHistory>()
            .Property(x => x.StartTime)
            .IsRequired();

        modelBuilder.Entity<ServiceHistory>()
            .Property(x => x.EndTime)
            .IsRequired();
        #endregion

        #region SimulationResult
        modelBuilder.Entity<SimulationResult>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<SimulationResult>()
            .HasOne(sr => sr.Device)
            .WithMany()
            .HasForeignKey(sr => sr.DeviceId);
        #endregion


        modelBuilder.Entity<EquipmentLoadForecast>()
            .HasKey(e => e.Id);
    }
}
