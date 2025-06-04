using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSale.Dto.Categories;
using WebSale.Dto.Feedbacks;
using WebSale.Dto.ProductDetails;
using WebSale.Dto.Products;
using WebSale.Dto.QueryDto;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Respository;

namespace WebSale.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFeedBackRepository _feedBackRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IImageFeedBackRepository _imageFeedBackRepository;

        public FeedBackController(IMapper mapper, IFeedBackRepository feedBackRepository, IUserRepository userRepository, IProductRepository productRepository, IImageFeedBackRepository imageFeedBackRepository)
        {
            _mapper = mapper;
            _feedBackRepository = feedBackRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _imageFeedBackRepository = imageFeedBackRepository;
        }

        [HttpGet("feedback")]
        public async Task<IActionResult> GetFeedBack([FromQuery] string inputUserId, [FromQuery] int productId, [FromQuery] int feedbackId )
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                if (!await _feedBackRepository.FeedBackExists(feedbackId))
                {
                    status.StatusCode = 400;
                    status.Message = "Feedback does not exists";
                    return BadRequest(status);
                }

                var feedback = await _feedBackRepository.GetFeedBack(productId, feedbackId);
                if (feedback == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting feedback";
                    return BadRequest(status);
                }
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("feedbacks")]
        public async Task<IActionResult> GetFeedBacks([FromQuery] string inputUserId, [FromQuery] int productId, [FromQuery] QueryPaginationDto queryPaginationDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var feedbacks = await _feedBackRepository.GetFeedBacks(productId, queryPaginationDto);
                if (feedbacks == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting order of user";
                    return BadRequest(status);
                }
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedBack([FromQuery] string inputUserId, [FromBody] FeedbackDto feedbackDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || feedbackDto == null)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                if (!await _productRepository.ProductExists(feedbackDto.ProductId))
                {
                    status.StatusCode = 400;
                    status.Message = "Product does not exists";
                    return BadRequest(status);
                }

                if (!await _userRepository.UserExists(userId))
                {
                    status.StatusCode = 400;
                    status.Message = "User does not exists";
                    return BadRequest(status);
                }
                var user = await _userRepository.GetUser(userId);
                var product = await _productRepository.GetProduct(feedbackDto.ProductId);
                var feedback = new FeedBack
                {
                    Content = feedbackDto.Content,
                    Rate = feedbackDto.Rate,
                    User = user,
                    Product = product,
                    CreatedAt = DateTime.Now
                };

                var newFeedback = await _feedBackRepository.CreateFeedBack(feedback);
                if (newFeedback == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating feedback";
                    return BadRequest(status);
                }

                if (feedbackDto.Urls != null && feedbackDto.Urls.Any())
                {
                    if (!await _imageFeedBackRepository.CreateImagesFeedBack(newFeedback.Id, feedbackDto.Urls))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while creating image feedback";
                        return BadRequest(status);
                    }
                }

                return Ok(await _feedBackRepository.GetFeedBack(product.Id, newFeedback.Id));
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeedBack([FromQuery] string inputUserId, [FromQuery] int feedbackId, [FromBody] FeedbackDto feedbackDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (feedbackDto == null || feedbackId == null || inputUserId != userId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please fill in all required into fields";
                    return BadRequest(status);
                }

                if (!await _feedBackRepository.FeedBackExists(feedbackId))
                {
                    status.StatusCode = 409;
                    status.Message = "Feedback does not exists";
                    return BadRequest(status);
                }
                if (!await _userRepository.UserExists(userId))
                {
                    status.StatusCode = 400;
                    status.Message = "User does not exists";
                    return BadRequest(status);
                }
                var user = await _userRepository.GetUser(userId);
                var product = await _productRepository.GetProduct(feedbackDto.ProductId);

                var feedback = await _feedBackRepository.GetFeedBackByFeedBackId(userId, feedbackId);
                feedback.Content = feedbackDto.Content;

                var feedbackUpdated = await _feedBackRepository.UpdateFeedBack(feedback);
                if (feedbackUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating feedback";
                    return BadRequest(status); ;
                }

                if (feedbackDto.Urls != null && feedbackDto.Urls.Any())
                {
                    if (!await _imageFeedBackRepository.UpdateImagesFeedBack(feedbackUpdated.Id, feedbackDto.Urls))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while updating image feedback";
                        return BadRequest(status);
                    }
                }

                return Ok(await _feedBackRepository.GetFeedBack(product.Id, feedbackUpdated.Id));
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFeedBack([FromQuery] string inputUserId, [FromQuery] int feedbackId)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (feedbackId == null || inputUserId != userId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please fill in all required into fields";
                    return BadRequest(status);
                }

                if (!await _feedBackRepository.FeedBackExists(feedbackId))
                {
                    status.StatusCode = 409;
                    status.Message = "Feedback does not exists";
                    return BadRequest(status);
                }
                if (!await _userRepository.UserExists(userId))
                {
                    status.StatusCode = 400;
                    status.Message = "User does not exists";
                    return BadRequest(status);
                }

                var feedback = await _feedBackRepository.GetFeedBackByFeedBackId(userId, feedbackId);
               
                var feedbackUpdated = await _feedBackRepository.DeleteFeedBack(feedback);
                if (feedbackUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while deleting feedback";
                    return BadRequest(status); ;
                }

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }
    }
}
