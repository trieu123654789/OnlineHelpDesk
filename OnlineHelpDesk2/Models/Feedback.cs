namespace OnlineHelpDesk2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feedback")]
    public partial class Feedback
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Feedback()
        {
            SummaryReports = new HashSet<SummaryReport>();
        }

        public int FeedbackID { get; set; }

        public int RequestID { get; set; }

        [Required]
        [StringLength(1000)]
        public string FeedbackContent { get; set; }

        public DateTime FeedbackTime { get; set; }

        public virtual Request Request { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SummaryReport> SummaryReports { get; set; }
    }
}
