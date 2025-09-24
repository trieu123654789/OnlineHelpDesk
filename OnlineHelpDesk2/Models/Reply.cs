namespace OnlineHelpDesk2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reply")]
    public partial class Reply
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reply()
        {
            SummaryReports = new HashSet<SummaryReport>();
        }

        public int ReplyID { get; set; }

        public int RequestID { get; set; }

        [Required]
        [StringLength(1000)]
        public string ReplyContent { get; set; }

        public DateTime ReplyDate { get; set; }

        public virtual Request Request { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SummaryReport> SummaryReports { get; set; }
    }
}
