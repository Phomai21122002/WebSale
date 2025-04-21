using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IImageFeedBackRepository
    {
        Task<ICollection<ImageFeedBack>> GetImageFeedBackByIdFeedBack(int idFeedBack);
        Task<bool> CreateImagesFeedBack(int idFeedBack, List<string> imagesFeedBack);
        Task<bool> UpdateImagesFeedBack(int idFeedBack, List<string> imagesFeedBack);
        Task<bool> DeleteImagesFeedBack(int idFeedBack, List<string> imagesFeedBack);
        Task<bool> ImageFeedBackExists(int idFeedBack, string imagesFeedBack);
    }
}
