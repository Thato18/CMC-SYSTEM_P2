using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMC_SYSTEM.Models
{
    public class ApprovalClaim
    {
        public int ApprovalId { get; set; }
        public int ClaimId { get; set; }
        public bool Approved { get; set; } = false; // Default to not approved
        public bool Rejected { get; set; } = false; // Default to not rejected
        public DateTime? ApprovalDate { get; set; } // Nullable to allow for unapproved claims
    }
}