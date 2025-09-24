using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace OnlineHelpDesk2.Models
{
    public class AccountList
    {
        public int AccountID { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public string TypeName { get; set; }

        public string FacilityName { get; set; }

        [Required]
        [StringLength(100)]
        public string Fullname { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; }

        public DateTime Birthday { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

      

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Requests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Requests1 { get; set; }
    }
}