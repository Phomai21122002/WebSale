using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WebSale.Dto.Products;
using WebSale.Dto.Categories;
using WebSale.Extensions;
using WebSale.Dto.QueryDto;
using WebSale.Dto.Feedbacks;

namespace WebSale.Respository
{
    public class FeedBackRepository : IFeedBackRepository
    {
        private readonly DataContext _dataContext;

        public FeedBackRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<FeedBack> CreateFeedBack(FeedBack feedback)
        {
            await _dataContext.AddAsync(feedback);
            await _dataContext.SaveChangesAsync();
            return feedback;
        }

        public async Task<FeedBack> DeleteFeedBack(FeedBack feedback)
        {
            _dataContext.Remove(feedback);
            await _dataContext.SaveChangesAsync();
            return feedback;
        }

        public async Task<bool> FeedBackExists(int id)
        {
            return await _dataContext.FeedBacks.AnyAsync(f => f.Id == id);
        }

        public async Task<FeedBack?> GetFeedBack(int productId, int feedbackId)
        {
            return await _dataContext.FeedBacks.Include(fb => fb.User).Include(fb => fb.ImageFeedBacks).FirstOrDefaultAsync(f => f.Id == feedbackId && f.Product != null && f.Product.Id == productId);
        }

        public async Task<FeedBack?> GetFeedBackByFeedBackId(string userId, int feedbackId)
        {
            return await _dataContext.FeedBacks.Include(fb => fb.User).Include(fb => fb.ImageFeedBacks).FirstOrDefaultAsync(f => f.Id == feedbackId && f.User != null && f.User.Id == userId);
        }

        public async Task<PageResult<ResultFeedbacksDto>> GetFeedBacks(int productId, QueryPaginationDto queryPaginationDto)
        {
            var query = _dataContext.FeedBacks.Where(fb => fb.Product != null && fb.Product.Id == productId).Include(fb => fb.User).Include(fb => fb.ImageFeedBacks).AsQueryable();

            var totalCount = await query.CountAsync();
            var starOne = await query.CountAsync(q => q.Rate == 1);
            var starTwo = await query.CountAsync(q => q.Rate == 2);
            var starThree = await query.CountAsync(q => q.Rate == 3);
            var starFour = await query.CountAsync(q => q.Rate == 4);
            var starFive = await query.CountAsync(q => q.Rate == 5);

            var totalStarCount = starOne + starTwo + starThree + starFour + starFive;
            var starSum = await query.SumAsync(q => q.Rate);
            float starAverage = totalStarCount > 0 ? (float)starSum / totalStarCount : 0f;

            if (queryPaginationDto.PageNumber > 0 && queryPaginationDto.PageSize > 0)
            {
                query = query
                    .Skip((queryPaginationDto.PageNumber - 1) * queryPaginationDto.PageSize)
                    .Take(queryPaginationDto.PageSize);
            }

            var resFeedbacks = await query.ToListAsync();

            var result = new ResultFeedbacksDto
            {
                StarOne = starOne,
                StarTwo = starTwo,
                StarThree = starThree,
                StarFour = starFour,
                StarFive = starFive,
                StarAverage = starAverage,
                feedBacks = resFeedbacks
            };

            return new PageResult<ResultFeedbacksDto>
            {
                TotalCount = totalCount,
                PageNumber = queryPaginationDto.PageNumber,
                PageSize = queryPaginationDto.PageSize,
                Datas = new List<ResultFeedbacksDto> { result }
            };
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<FeedBack> UpdateFeedBack(FeedBack feedback)
        {
            _dataContext.Update(feedback);
            await _dataContext.SaveChangesAsync();
            return feedback;
        }
    }
}
