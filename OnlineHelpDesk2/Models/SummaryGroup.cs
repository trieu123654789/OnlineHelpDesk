using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineHelpDesk2.Models
{
    public class SummaryGroup
    {

        public int TotalRequests { get; set; }
        public List<Report> Reports { get; set; }
        public int LowRequests { get; set; }
        public int MediumRequests { get; set; }
        public int HeavyRequests { get; set; }
        public List<Request> Request { get; set; }

    }
}