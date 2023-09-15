using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace EC.EntityFrameworkCore
{
    public static class ECDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ECDbContext> builder, string connectionString)
        {
            builder.UseNpgsql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ECDbContext> builder, DbConnection connection)
        {
            builder.UseNpgsql(connection);
        }
    }
}
