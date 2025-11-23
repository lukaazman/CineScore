using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace CineScore.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int MovieId { get; set; }

        public User? User { get; set; }
        public Movie? Movie { get; set; }
    }
}