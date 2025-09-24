using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineHelpDesk2.Models
{
    public class Reason
    {
        public int RequestID { get; set; }
        public int RequestorID { get; set; }
        public string RequestorFullName { get; set; }
        public string FacilityName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string RequestContent { get; set; }
        public string SeverityLevels { get; set; }
        public string ReasonDescription { get; set; }
    }
}