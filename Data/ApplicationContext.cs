// File: Data/ApplicationContext.cs

using Entity; // Make sure your Entity namespace is included
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Keep if you use OnConfiguring for local development
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Keep if you use MySQL

namespace Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; } // ⬅️ Ensure this DbSet is present
        public DbSet<Department> Departments { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Shifts> Shifts { get; set; }
        public DbSet<LifeCycleTask> LifeCycleTasks { get; set; }
        public DbSet<JobOpenings> JobOpenings { get; set; }
        public DbSet<Refferals> Refferals { get; set; }
        public DbSet<Candidates> Candidates { get; set; }

        public ApplicationContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This configuration should now be compatible after changing User.EmpId to int?
            modelBuilder.Entity<Employee>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId) // Employee.UserId (int) FK to User.UserId (int)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne<Employee>()
                .WithOne()
                .HasForeignKey<User>(u => u.EmpId) // User.EmpId (int?) FK to Employee.EmpId (int)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull); // Or your preferred cascade behavior

            modelBuilder.Entity<AttendanceRecord>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ar => ar.UserId) // AttendanceRecord.UserId (int) FK to User.UserId (int)
                .IsRequired();
        }
    }
}
