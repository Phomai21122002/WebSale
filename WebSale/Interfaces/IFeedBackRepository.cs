using WebSale.Models;

namespace WebSale.Interfaces
{
    public interface IFeedBackRepository
    {
        Task<ICollection<FeedBack>> GetFeedBacks(int productId);
        Task<FeedBack?> GetFeedBackByFeedBackId(string userId, int feedbackId);
        Task<FeedBack?> GetFeedBack(int productId, int feedbackId);
        Task<FeedBack> CreateFeedBack(FeedBack feedback);
        Task<FeedBack> UpdateFeedBack(FeedBack feedback);
        Task<FeedBack> DeleteFeedBack(FeedBack feedback);
        Task<bool> Save();
        Task<bool> FeedBackExists(int id);

    }
}
