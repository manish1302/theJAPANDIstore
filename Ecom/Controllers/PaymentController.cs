using Ecom.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;

namespace Ecom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {

        private readonly string merchantKey = "z5JyD4";
        private readonly string salt = "GG9BQCZ13PzrUSdO8BhXjchQeEz3wCI4";

        [HttpPost("paymentInitiate")]
        public IActionResult InitiatePayment([FromBody] PaymentModel request)
        {
            string txnid = GenerateTxnId(); // generate a unique transaction ID
            string amount = request.Amount.ToString("F2"); // Format to two decimal places
            string productInfo = request.ProductInfo;
            string firstname = request.FirstName;
            string email = request.Email;
            string phone = request.Phone;

            string hashString = $"{merchantKey}|{txnid}|{amount}|{productInfo}|{firstname}|{email}|||||||||||{salt}";
            string hash = GenerateHash512(hashString);

            var payuRequest = new
            {
                key = merchantKey,
                txnid = txnid,
                amount = amount,
                productinfo = productInfo,
                firstname = firstname,
                email = email,
                phone = phone,
                surl = "http://localhost:7272/api/Payment/success", // Success URL
                furl = "http://localhost:7272/api/Payment/failure", // Failure URL
                hash = hash
            };

            return Ok(payuRequest);
        }

        [HttpPost("success")]
        public IActionResult Success([FromForm] PaymentResponseModel payuResponse)
        {
            string txnid = payuResponse.txnid;
            string status = payuResponse.status;
            string amount = payuResponse.amount;
            string paymentMode = payuResponse.mode;
            bool isHashValid = VerifyPaymentHash(payuResponse);

            if (isHashValid && status == "success")
            {
                // Handle successful payment, update database, etc.
                return Ok("Payment successful");
            }
            else
            {
                return BadRequest("Invalid Payment Response");
            }
        }

        [HttpPost("failure")]
        public IActionResult PaymentFailure([FromForm] PaymentResponseModel payuResponse)
        {
            // Extract necessary details from the PayU response
            string txnid = payuResponse.txnid;
            string status = payuResponse.status;
            string error = payuResponse.error_Message;

            // Handle failed payment
            return BadRequest("Payment failed: " + error);
        }
        private static string GenerateHash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);
            using (var hashString = SHA512.Create())
            {
                byte[] hashValue = hashString.ComputeHash(message);
                StringBuilder hex = new StringBuilder(hashValue.Length * 2);
                foreach (byte x in hashValue)
                {
                    hex.AppendFormat("{0:x2}", x);
                }
                return hex.ToString();
            }
        }

        private static string GenerateTxnId()
        {
            // Use Guid to create a unique identifier, and get the current timestamp
            string guidPart = Guid.NewGuid().ToString("N"); // Generates a 32-character unique string without dashes
            string timeStampPart = DateTime.UtcNow.Ticks.ToString(); // Get the current timestamp in ticks

            // Combine both parts to ensure the transaction ID is unique
            return guidPart + timeStampPart;
        }

        private bool VerifyPaymentHash(PaymentResponseModel payuResponse)
        {
            // Step 1: Extract the parameters from the PayU response
            string status = payuResponse.status;
            string txnid = payuResponse.txnid;
            string amount = payuResponse.amount;
            string productInfo = payuResponse.productinfo;
            string firstname = payuResponse.firstname;
            string email = payuResponse.email;
            string receivedHash = payuResponse.hash;

            // Step 2: Reconstruct the hash string based on the PayU response format
            string hashString = $"{salt}|{status}|||||||||||{email}|{firstname}|{productInfo}|{amount}|{txnid}|{merchantKey}";

            // Step 3: Generate the hash using SHA512
            string generatedHash = GenerateHash512(hashString);

            // Step 4: Compare the generated hash with the received hash
            return string.Equals(generatedHash, receivedHash, StringComparison.OrdinalIgnoreCase);
        }

    }
}
