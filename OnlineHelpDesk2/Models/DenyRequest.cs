using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineHelpDesk2.Models
{
    public class DenyRequest
    {
        public int RequestID { get; set; }
        public int RequestorID { get; set; }
        public string RequestorFullName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string RequestContent { get; set; }
        public string SeverityLevels { get; set; }
        public string Reason { get; set; }
    }
}