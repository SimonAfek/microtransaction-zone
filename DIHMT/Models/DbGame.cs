//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class DbGame
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DbGame()
        {
            this.DbGamePlatforms = new HashSet<DbGamePlatform>();
            this.DbGameGenres = new HashSet<DbGameGenre>();
            this.DbGameRatings = new HashSet<DbGameRating>();
            this.PendingSubmissions = new HashSet<PendingSubmission>();
            this.DbGameLinks = new HashSet<DbGameLink>();
            this.ThumbImages = new HashSet<ThumbImage>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string RatingExplanation { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public string SmallImageUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        public string GbSiteDetailUrl { get; set; }
        public bool IsRated { get; set; }
        public Nullable<System.DateTime> RatingLastUpdated { get; set; }
        public string Basically { get; set; }
        public string Aliases { get; set; }
        public Nullable<System.DateTime> ReleaseDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DbGamePlatform> DbGamePlatforms { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DbGameGenre> DbGameGenres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DbGameRating> DbGameRatings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PendingSubmission> PendingSubmissions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DbGameLink> DbGameLinks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThumbImage> ThumbImages { get; set; }
    }
}
