﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DIHMT.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DIHMTEntities : DbContext
    {
        public DIHMTEntities()
            : base("name=DIHMTEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DbGame> DbGames { get; set; }
        public virtual DbSet<DbGamePlatform> DbGamePlatforms { get; set; }
        public virtual DbSet<DbPlatform> DbPlatforms { get; set; }
        public virtual DbSet<DbGameGenre> DbGameGenres { get; set; }
        public virtual DbSet<DbGenre> DbGenres { get; set; }
        public virtual DbSet<DbGameRating> DbGameRatings { get; set; }
        public virtual DbSet<DbRating> DbRatings { get; set; }
        public virtual DbSet<PendingDbGameRating> PendingDbGameRatings { get; set; }
        public virtual DbSet<PendingSubmission> PendingSubmissions { get; set; }
        public virtual DbSet<DbGameLink> DbGameLinks { get; set; }
        public virtual DbSet<PendingGameLink> PendingGameLinks { get; set; }
        public virtual DbSet<BlockList> BlockLists { get; set; }
    }
}
