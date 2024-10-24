using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMC_SYSTEM.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string LecturerName { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public HttpPostedFileBase SupportingDocument { get; set; } // Use HttpPostedFileBase for file uploads
    }
}
