using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using CMC_SYSTEM.Models;

namespace CMC_SYSTEM.Controllers
{
    public class HomeController : Controller
    {
        // Define the connection string
        private string connectionString = "Data Source=NTUZUMA\\SQLEXPRESS;Initial Catalog=cmcDb;Integrated Security=True";

        // Action for the Index page
        public ActionResult Index()
        {
            return View(); // Make sure to create an Index.cshtml view
        }
        public ActionResult About()
        {
            return View(); // Make sure you have About.cshtml
        }
        public ActionResult Contact()
        {
            return View(); // Make sure you have Contact.cshtml
        }

        public ActionResult SubmitClaim()
        {
            ViewBag.Message = "Submit your claim here.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitClaim(Claim claim)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "INSERT INTO Claims (LecturerName, HoursWorked, HourlyRate, SupportingDocument) " +
                                       "VALUES (@LecturerName, @HoursWorked, @HourlyRate, @SupportingDocument)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@LecturerName", claim.LecturerName);
                            cmd.Parameters.AddWithValue("@HoursWorked", claim.HoursWorked);
                            cmd.Parameters.AddWithValue("@HourlyRate", claim.HourlyRate);

                            if (claim.SupportingDocument != null)
                            {
                                byte[] fileData;
                                using (var binaryReader = new System.IO.BinaryReader(claim.SupportingDocument.InputStream))
                                {
                                    fileData = binaryReader.ReadBytes(claim.SupportingDocument.ContentLength);
                                }
                                cmd.Parameters.AddWithValue("@SupportingDocument", fileData);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@SupportingDocument", DBNull.Value);
                            }

                            // Debug line to log values before execution
                            System.Diagnostics.Debug.WriteLine($"LecturerName: {claim.LecturerName}, HoursWorked: {claim.HoursWorked}, HourlyRate: {claim.HourlyRate}");

                            cmd.ExecuteNonQuery(); // This line performs the insert
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log detailed error message
                        System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                        ModelState.AddModelError("", "Unable to submit claim. " + ex.Message);
                        return View(claim);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(claim);
        }

        // Action to view submitted claims for approval
        // Action to view submitted claims for approval
        public ActionResult ApproveClaim()
        {
            List<ClaimApproval> claims = new List<ClaimApproval>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Join the Claims and ClaimApprovals tables to get claim data along with approval status
                string query = @"
        SELECT c.ClaimId, c.LecturerName, c.HoursWorked, c.HourlyRate, ca.Status
        FROM Claims c
        JOIN ClaimApprovals ca ON c.ClaimId = ca.ClaimId
        WHERE ca.Status = 'Pending'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            claims.Add(new ClaimApproval
                            {
                                ClaimId = reader.GetInt32(0),
                                LecturerName = reader.GetString(1),
                                HoursWorked = reader.GetDecimal(2),
                                HourlyRate = reader.GetDecimal(3),
                                Status = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return View(claims); // Pass the list of claims to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveClaim(int ClaimId, string action)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = string.Empty;

                // Update the query based on the action ("approve" or "reject")
                if (action == "approve")
                {
                    query = "UPDATE ClaimApprovals SET Status = 'Approved', ApprovalDate = @ApprovalDate WHERE ClaimId = @ClaimId";
                }
                else if (action == "reject")
                {
                    query = "UPDATE ClaimApprovals SET Status = 'Rejected', ApprovalDate = @ApprovalDate WHERE ClaimId = @ClaimId";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Pass the ClaimId and current date as parameters
                    cmd.Parameters.AddWithValue("@ClaimId", ClaimId);
                    cmd.Parameters.AddWithValue("@ApprovalDate", DateTime.Now);

                    cmd.ExecuteNonQuery(); // Execute the update query
                }
            }
            // After approval or rejection, return to the list of pending claims
            return RedirectToAction("ApproveClaim");
        }

        public ActionResult ViewClaims()
        {
            List<Claim> claimsList = GetClaimsFromDatabase();
            return View(claimsList);
        }

        private List<Claim> GetClaimsFromDatabase()
        {
            List<Claim> claimsList = new List<Claim>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ClaimId, LecturerName, HoursWorked, HourlyRate FROM Claims"; // Adjust query based on your DB schema
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Claim claim = new Claim
                            {
                                ClaimId = Convert.ToInt32(dr["ClaimId"]),
                                LecturerName = dr["LecturerName"].ToString(),
                                HoursWorked = Convert.ToDecimal(dr["HoursWorked"]),
                                HourlyRate = Convert.ToDecimal(dr["HourlyRate"]),
                                // SupportingDocument is not retrieved here since it's a file upload, typically handled differently.
                            };
                            claimsList.Add(claim);
                        }
                    }
                }
            }

            return claimsList;
        }
    }
}