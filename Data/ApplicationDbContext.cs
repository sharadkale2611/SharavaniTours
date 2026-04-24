using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Models;
using System.Linq.Expressions;

namespace SharavaniTours.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


		public DbSet<Client> Clients { get; set; }
		public DbSet<Vehicle> Vehicles { get; set; }
		public DbSet<RateCard> RateCards { get; set; }
		public DbSet<Trip> Trips { get; set; }
		public DbSet<DutySlip> DutySlips { get; set; }
		public DbSet<ClientUser> ClientUsers { get; set; }
		public DbSet<VehicleType> VehicleTypes { get; set; }
		




		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// ✅ STEP : Global Soft Delete Filter
			foreach (var entityType in builder.Model.GetEntityTypes())
			{
				if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
				{
					builder.Entity(entityType.ClrType)
						.HasQueryFilter(GenerateIsDeletedFilter(entityType.ClrType));
				}
			}

			builder.Entity<Trip>(entity =>
			{
				entity.HasOne(t => t.RequestedVehicle)
					.WithMany()
					.HasForeignKey(t => t.RequestedVehicleId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(t => t.SentVehicle)
					.WithMany()
					.HasForeignKey(t => t.SentVehicleId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Trip>()
				.HasOne(t => t.Driver)
				.WithMany()
				.HasForeignKey(t => t.DriverId)
				.OnDelete(DeleteBehavior.Restrict); 

			builder.Entity<Trip>()
				.HasOne(t => t.Client)
				.WithMany()
				.HasForeignKey(t => t.ClientId)
				.OnDelete(DeleteBehavior.Restrict); 
		}

		public override int SaveChanges()
		{
			var entries = ChangeTracker.Entries<BaseEntity>();

			foreach (var entry in entries)
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedAt = DateTime.Now;
				}
				else if (entry.State == EntityState.Modified)
				{
					entry.Entity.UpdatedAt = DateTime.Now;
				}
			}

			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var entries = ChangeTracker.Entries<BaseEntity>();

			foreach (var entry in entries)
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedAt = DateTime.Now;
				}
				else if (entry.State == EntityState.Modified)
				{
					entry.Entity.UpdatedAt = DateTime.Now;
				}
			}

			return await base.SaveChangesAsync(cancellationToken);
		}

		private static LambdaExpression GenerateIsDeletedFilter(Type type)
		{
			var param = Expression.Parameter(type, "e");
			var prop = Expression.Property(param, "IsDeleted");
			var condition = Expression.Equal(prop, Expression.Constant(false));
			return Expression.Lambda(condition, param);
		}

	}
}
