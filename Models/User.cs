using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace CineScore.Models
{
    public class User : IdentityUser
    {
        //public int Id { get; set; }
        //public string Username { get; set; } = "";
        //public string Email { get; set; } = "";
        //public string Password { get; set; } = "";

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
        //public ICollection<Watchlist>? Watchlist { get; set; }
    }
}
