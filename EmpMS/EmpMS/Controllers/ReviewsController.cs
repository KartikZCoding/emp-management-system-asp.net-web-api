using Application.Common;
using Application.DTOs.Review;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IPerformanceReviewService _reviewService;

        public ReviewsController(IPerformanceReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Create a performance review for an employee
        /// </summary>
        [HttpPost]
        [HasPermission("Review.Create")]
        [ProducesResponseType(typeof(APIResponse<ReviewResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<ReviewResponseDto>>> CreateReview(CreateReviewDto dto)
        {
            var reviewerId = Convert.ToInt32(User.FindFirst("EmployeeId")?.Value);
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var response = await _reviewService.CreateReviewAsync(dto, reviewerId, username);

            return StatusCode(StatusCodes.Status201Created, new APIResponse<ReviewResponseDto>(response)
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Performance review created successfully"
            });
        }

        /// <summary>
        /// Get all reviews for a specific employee
        /// </summary>
        [HttpGet("employee/{empId}")]
        [HasPermission("Review.Read")]
        [ProducesResponseType(typeof(APIResponse<List<ReviewResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<List<ReviewResponseDto>>>> GetEmployeeReviews(int empId)
        {
            var response = await _reviewService.GetEmployeeReviewsAsync(empId);
            return Ok(new APIResponse<List<ReviewResponseDto>>(response));
        }

        /// <summary>
        /// Get own performance reviews (authenticated employee)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(APIResponse<List<ReviewResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<List<ReviewResponseDto>>>> GetMyReviews()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _reviewService.GetMyReviewsAsync(email);
            return Ok(new APIResponse<List<ReviewResponseDto>>(response));
        }

        /// <summary>
        /// Update an existing performance review
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission("Review.Update")]
        [ProducesResponseType(typeof(APIResponse<ReviewResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<ReviewResponseDto>>> UpdateReview(int id, UpdateReviewDto dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var response = await _reviewService.UpdateReviewAsync(id, dto, username);
            return Ok(new APIResponse<ReviewResponseDto>(response));
        }

        /// <summary>
        /// Delete a performance review (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission("Review.Delete")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteReview(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return Ok(new APIResponse { Message = "Performance review deleted successfully" });
        }

        /// <summary>
        /// Get department-wide review summary for a given year
        /// </summary>
        [HttpGet("department/{deptId}")]
        [HasPermission("Review.Read")]
        [ProducesResponseType(typeof(APIResponse<DepartmentReviewSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<DepartmentReviewSummaryDto>>> GetDepartmentSummary(
            int deptId, [FromQuery] int year)
        {
            var response = await _reviewService.GetDepartmentReviewSummaryAsync(deptId, year);
            return Ok(new APIResponse<DepartmentReviewSummaryDto>(response));
        }
    }
}
