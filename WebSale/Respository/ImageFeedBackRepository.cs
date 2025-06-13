using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebSale.Data;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Respository
{
    public class ImageFeedBackRepository : ImageRepositoryBase<ImageFeedBack>, IImageFeedBackRepository
    {
        public ImageFeedBackRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper, dataContext.ImageFeedBacks) { }
        public async Task<bool> CreateImagesFeedBack(int idFeedBack, List<string> imagesFeedBack)
        {
            return await CreateImages(idFeedBack, imagesFeedBack,   
                async id => await _dataContext.FeedBacks.FindAsync(id),
                (url, feedback) => new ImageFeedBack { Url =  url, FeedBack = (FeedBack)feedback });
        }

        public async Task<bool> DeleteImagesFeedBack(ICollection<ImageFeedBack> imagesFeedback)
        {
            if (imagesFeedback.Count <= 0)
            {
                return true;
            }
            foreach (var image in imagesFeedback)
            {
                _dataContext.ImageFeedBacks.Remove(image);
            }
            return await Save();
        }

        public async Task<ICollection<ImageFeedBack>> GetImageFeedBackByIdFeedBack(int idFeedBack)
        {
            return await _dataContext.ImageFeedBacks.Where(fb => fb.FeedBack != null && fb.FeedBack.Id == idFeedBack).Include(fb => fb.FeedBack).ToListAsync();
        }

        public Task<bool> ImageFeedBackExists(int idFeedBack, string imagesFeedBack)
        {
            throw new NotImplementedException();
        }
        public new async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateImagesFeedBack(int idFeedBack, List<string> imagesFeedBack)
        {
            var imageOfFeedBack = await GetImageFeedBackByIdFeedBack(idFeedBack);
            return await UpdateImages(idFeedBack, imagesFeedBack, imageOfFeedBack,
                async id => await _dataContext.FeedBacks.FindAsync(id),    
                (url, feedback) => new ImageFeedBack{ Url = url, FeedBack = (FeedBack)feedback },
                img => img.Url);
        }
    }
}
