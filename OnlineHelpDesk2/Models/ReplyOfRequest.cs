using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineHelpDesk2.Models
{
    public class ReplyOfAssignee
    {
        public int ReplyID { get; set; }
        public string ReplyContent { get; set; }
        public DateTime ReplyDate { get; set; }
        public int RequestID { get; set; }
        public int RequestorID { get; set; }
        public string RequestorFullName { get; set; }
        public string FacilityName { get; set; }
        public DateTime RequestDate { get; set; }
        public int AssigneeID { get; set; }
        public string AssigneeFullName { get; set; }
        public string Status { get; set; }
        public string RequestContent { get; set; }
        public string SeverityLevels { get; set; }
    }
}