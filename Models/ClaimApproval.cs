using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMC_SYSTEM.Models
{
    public class ClaimApproval
    {
        public int ApprovalId { get; set; }
        public int ClaimId { get; set; }
        public string LecturerName { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Status { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}