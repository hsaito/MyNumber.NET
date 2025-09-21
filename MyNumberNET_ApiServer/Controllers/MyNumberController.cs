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
        /// Verifies and creates a MyNumberValue from the provided input string.
        /// </summary>
        /// <param name="numberString">A string representation of the My Number (may include separators).</param>
        /// <returns>A MyNumberValue object if valid, or BadRequest if invalid.</returns>
        [HttpPost("verify-string")]
        public ActionResult<MyNumberValue> VerifyString([FromBody] string numberString)
        {
            try
            {
                if (MyNumberValue.TryParse(numberString, out var myNumber))
                {
                    return Ok(myNumber);
                }
                return BadRequest($"Invalid My Number format: {numberString}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a MyNumberValue from the provided digits array.
        /// </summary>
        /// <param name="number">An array of 12 integers representing the My Number digits.</param>
        /// <returns>A MyNumberValue object if valid, or BadRequest if invalid.</returns>
        [HttpPost("create")]
        public ActionResult<MyNumberValue> Create([FromBody] int[] number)
        {
            try
            {
                var myNumber = new MyNumberValue(number);
                return Ok(myNumber);
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

        /// <summary>
        /// Creates a complete MyNumberValue from the first 11 digits by calculating the check digit.
        /// </summary>
        /// <param name="number">An array of 11 integers representing the first 11 digits of My Number.</param>
        /// <returns>A complete MyNumberValue with calculated check digit, or BadRequest if invalid.</returns>
        [HttpPost("complete")]
        public ActionResult<MyNumberValue> Complete([FromBody] int[] number)
        {
            try
            {
                var myNumber = MyNumberValue.FromFirstElevenDigits(number);
                return Ok(myNumber);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Generates a random valid My Number.
        /// </summary>
        /// <returns>A randomly generated MyNumberValue.</returns>
        [HttpGet("generate")]
        public ActionResult<MyNumberValue> Generate()
        {
            try
            {
                var myNumber = MyNumberValue.GenerateRandom();
                return Ok(myNumber);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Formats a MyNumberValue with the specified format.
        /// </summary>
        /// <param name="request">Request containing the My Number string and desired format.</param>
        /// <returns>The formatted My Number string, or BadRequest if invalid.</returns>
        [HttpPost("format")]
        public ActionResult<string> Format([FromBody] FormatRequest request)
        {
            try
            {
                if (MyNumberValue.TryParse(request.Number, out var myNumber))
                {
                    var formatted = myNumber.ToString(request.Format ?? "N");
                    return Ok(formatted);
                }
                return BadRequest($"Invalid My Number format: {request.Number}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    /// <summary>
    /// Request model for formatting operations.
    /// </summary>
    public class FormatRequest
    {
        /// <summary>
        /// The My Number to format.
        /// </summary>
        public string Number { get; set; } = "";

        /// <summary>
        /// The format to apply ("N", "S", "H", "G").
        /// </summary>
        public string? Format { get; set; }
    }
}
