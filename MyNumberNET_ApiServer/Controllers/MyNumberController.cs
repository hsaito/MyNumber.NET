using Microsoft.AspNetCore.Mvc;
using MyNumberNET;

namespace MyNumberNET_ApiServer.Controllers
{
    /// <summary>
    /// Controller for exposing MyNumberNET functionality via REST API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MyNumberController : ControllerBase
    {
        /// <summary>
        /// Verifies if the provided 12-digit array is a valid "My Number".
        /// </summary>
        /// <param name="number">An array of 12 integers representing the My Number digits.</param>
        /// <returns>True if valid, false otherwise. Returns BadRequest if input is malformed.</returns>
        [HttpPost("verify")]
        public ActionResult<bool> Verify([FromBody] int[] number)
        {
            try
            {
                bool isValid = MyNumber.VerifyNumber(number);
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Calculates the check digit for the provided 11-digit array.
        /// </summary>
        /// <param name="number">An array of 11 integers representing the first 11 digits of My Number.</param>
        /// <returns>The calculated check digit. Returns BadRequest if input is malformed.</returns>
        [HttpPost("checkdigit")]
        public ActionResult<int> CheckDigit([FromBody] int[] number)
        {
            try
            {
                int checkDigit = MyNumber.CalculateCheckDigits(number);
                return Ok(checkDigit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
