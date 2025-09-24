namespace OnlineHelpDesk2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GuestLetter")]
    public partial class GuestLetter
    {
        [Key]
        public int LetterID { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Request Content")]
        public string LetterContent { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Mail { get; set; }
    }
}
