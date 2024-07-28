using Microsoft.EntityFrameworkCore;
using RL.Data.DataModels;
using RL.Data.DataModels.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RL.Data
{
    public class RLContext : DbContext
    {
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<PlanProcedure> PlanProcedures { get; set; }
        public DbSet<PlanProcedureUser> PlanProcedureUsers { get; set; }
        public DbSet<User> Users { get; set; }

        public RLContext(DbContextOptions<RLContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PlanProcedure>(typeBuilder =>
            {
                typeBuilder.HasKey(pp => new { pp.PlanId, pp.ProcedureId });
                typeBuilder.HasOne(pp => pp.Plan)
                           .WithMany(p => p.PlanProcedures)
                           .HasForeignKey(pp => pp.PlanId);
                typeBuilder.HasOne(pp => pp.Procedure)
                           .WithMany()
                           .HasForeignKey(pp => pp.ProcedureId);
                typeBuilder.HasMany(pp => pp.PlanProcedureUsers)
                           .WithOne(ppu => ppu.PlanProcedure)
                           .HasForeignKey(ppu => new { ppu.PlanId, ppu.ProcedureId });
            });

            builder.Entity<PlanProcedureUser>(typeBuilder =>
            {
                typeBuilder.HasKey(ppu => new { ppu.PlanId, ppu.ProcedureId, ppu.UserId });
                typeBuilder.HasOne(ppu => ppu.PlanProcedure)
                           .WithMany(pp => pp.PlanProcedureUsers)
                           .HasForeignKey(ppu => new { ppu.PlanId, ppu.ProcedureId });
                typeBuilder.HasOne(ppu => ppu.User)
                           .WithMany()
                           .HasForeignKey(ppu => ppu.UserId);
            });
        }

        #region TimeStamps
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IChangeTrackable && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IChangeTrackable)entity.Entity).CreateDate = DateTime.UtcNow;
                }

                ((IChangeTrackable)entity.Entity).UpdateDate = DateTime.UtcNow;
            }
        }
        #endregion
    }
}
