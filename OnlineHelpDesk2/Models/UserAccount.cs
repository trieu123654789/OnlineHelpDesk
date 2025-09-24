using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace OnlineHelpDesk2.Models
{
    public class UserAccount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserAccount()
        {
            Requests = new HashSet<Request>();
            Requests1 = new HashSet<Request>();
        }

        public int AccountID { get; set; }


        [Required(ErrorMessage = "Usename is required")]
        [StringLength(50, ErrorMessage = "Username must be at most 50 characters long.")]
        [Index(IsUnique = true)] // Ensure uniqueness at the database level
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [Index(IsUnique = true)] // Ensure uniqueness at the database level
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(256, ErrorMessage = "Email must be at most 256 characters long.")]
        public string Email { get; set; }

        public int TypeID { get; set; }

        public int FacilityID { get; set; }

        [Required(ErrorMessage = "Fullname is required")]
        [StringLength(100)]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(20)]
        public string Gender { get; set; }

        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Invalid phone number, Phone must start with 0 and have 10 digits.")]
        [StringLength(10)]
        public string Phone { get; set; }

        public virtual Facility Facility { get; set; }

        public virtual UserType UserType { get; set; }

        [Display(Name = "User Type")]
        public string UserTypeName => UserType?.TypeName;

        [Display(Name = "Facility")]
        public string FacilityName => Facility?.FacilityName;

        [NotMapped]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase UploadFile { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Requests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Requests1 { get; set; }
    }
}