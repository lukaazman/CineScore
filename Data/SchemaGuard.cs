using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CineScore.Data
{
    public static class SchemaGuard
    {
        public static async Task EnsureBannerColumnAsync(DbContext context)
        {
            const string sql = @"IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Movies' AND COLUMN_NAME = 'BannerUrl')
BEGIN
    ALTER TABLE Movies ADD BannerUrl nvarchar(max) NULL;
END";

            await context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
