using Microsoft.EntityFrameworkCore;
using CineScore.Models;

namespace CineScore.Data
{
    public class CineScoreContext : DbContext
    {
        public CineScoreContext(DbContextOptions<CineScoreContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}
