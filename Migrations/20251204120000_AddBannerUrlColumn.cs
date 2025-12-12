using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineScore.Migrations
{
    public partial class AddBannerUrlColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF COL_LENGTH('dbo.Movies', 'BannerUrl') IS NULL
BEGIN
    ALTER TABLE [dbo].[Movies] ADD [BannerUrl] nvarchar(max) NULL;
END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF COL_LENGTH('dbo.Movies', 'BannerUrl') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Movies] DROP COLUMN [BannerUrl];
END");
        }
    }
}
