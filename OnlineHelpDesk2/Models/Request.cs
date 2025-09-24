namespace OnlineHelpDesk2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Request()
        {
            CloseRequests = new HashSet<CloseRequest>();
            Feedbacks = new HashSet<Feedback>();
            Replies = new HashSet<Reply>();
            SummaryReports = new HashSet<SummaryReport>();
        }

        public int RequestID { get; set; }

        public int RequestorID { get; set; }

        public int FacilityID { get; set; }

        public DateTime RequestDate { get; set; }

        public int? AssigneeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000)]
        public string RequestContent { get; set; }

        [Required]
        [StringLength(20)]
        public string SeverityLevels { get; set; }

        public virtual Account Account { get; set; }

        public virtual Account Account1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CloseRequest> CloseRequests { get; set; }

        public virtual Facility Facility { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reply> Replies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SummaryReport> SummaryReports { get; set; }
    }
}
