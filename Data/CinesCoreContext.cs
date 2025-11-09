using Microsoft.EntityFrameworkCore;
using CineScore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CineScore.Data
{
    public class CineScoreContext : IdentityDbContext<User>
    {
        public CineScoreContext(DbContextOptions<CineScoreContext> options)
            : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}
