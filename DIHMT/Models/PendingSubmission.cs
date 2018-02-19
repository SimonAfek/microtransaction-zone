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
    
    public partial class PendingSubmission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PendingSubmission()
        {
            this.PendingDbGameRatings = new HashSet<PendingDbGameRating>();
        }
    
        public int Id { get; set; }
        public int GameId { get; set; }
        public string RatingExplanation { get; set; }
        public System.DateTime TimeOfSubmission { get; set; }
        public string SubmitterIp { get; set; }
    
        public virtual DbGame DbGame { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PendingDbGameRating> PendingDbGameRatings { get; set; }
    }
}
