namespace OnlineHelpDesk2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CloseRequest")]
    public partial class CloseRequest
    {
        public int CloseRequestID { get; set; }

        public int RequestID { get; set; }

        [Required]
        [StringLength(300)]
        public string Reason { get; set; }

        public virtual Request Request { get; set; }
    }
}
