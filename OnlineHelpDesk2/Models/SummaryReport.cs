namespace OnlineHelpDesk2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SummaryReport")]
    public partial class SummaryReport
    {
        public int SummaryReportID { get; set; }

        public int ReportMonth { get; set; }

        public int RequestID { get; set; }

        public int ReplyID { get; set; }

        public int FeedbackID { get; set; }

        public virtual Feedback Feedback { get; set; }

        public virtual Reply Reply { get; set; }

        public virtual Report Report { get; set; }

        public virtual Request Request { get; set; }
    }
}
