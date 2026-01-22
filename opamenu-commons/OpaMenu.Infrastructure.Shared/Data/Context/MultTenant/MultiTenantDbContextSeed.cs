using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Data.Context.MultTenant
{
    public static class MultiTenantDbContextSeed
    {
        private static readonly DateTime _seedDate = new(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc);
        public static void MultiTenantSeed(this ModelBuilder modelBuilder)
        {
            // Seed Plans
            modelBuilder.Entity<PlanEntity>().HasData(
                new PlanEntity
                {
                    Id = GenerateId($"PLAN_free"),
                    Name = "Free",
                    Slug = "free",
                    Description = "Plano gratuito para pequenos negócios",
                    Price = 0m,
                    MaxUsers = 1,
                    CreatedAt = _seedDate,
                    UpdatedAt = null,
                },
                new PlanEntity
                {
                    Id = GenerateId($"PLAN_pro"),
                    Name = "Pro",
                    Slug = "pro",
                    Description = "Plano profissional para negócios em crescimento",
                    Price = 99.90m,
                    MaxUsers = 10,
                    CreatedAt = _seedDate,
                    UpdatedAt = null,
                }
            );
        }
        private static Guid GenerateId(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return new Guid(hash);
            }
        }
    }
}
