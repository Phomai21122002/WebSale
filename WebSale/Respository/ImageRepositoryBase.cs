using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSale.Data;
using WebSale.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebSale.Respository
{
    public abstract class ImageRepositoryBase<T> where T : class
    {
        protected readonly DataContext _dataContext;
        protected readonly IMapper _mapper;

        private readonly DbSet<T> _dbSet;

        protected ImageRepositoryBase(DataContext dataContext, IMapper mapper, DbSet<T> dbSet)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _dbSet = dbSet;
        }

        protected async Task<bool> CreateImages(int id, List<string> images, Func<int, Task<object>> findEntity, Func<string, object, T> createImage)
        {
            try
            {
                var entity = await findEntity(id);
                if (entity == null) return false;

                var imageEntities = images.Select(url => createImage(url, entity));
                await _dataContext.AddRangeAsync(imageEntities);
                return await Save();
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected async Task<bool> UpdateImages(int id, List<string> images, ICollection<T> imageOfEntity, Func<int, Task<object>> findEntity, Func<string, object, T> createImages, Func<T, string> getUrl)
        {
            try
            {
                Console.WriteLine("UpdateImages");
                var entiry = await findEntity(id);

                if (entiry == null) return false;

                var imageToRemove = imageOfEntity.Where(img => !images.Contains(getUrl(img))).ToList();

                var imageToAdd = images.Where(url => !imageOfEntity.Any(img => getUrl(img).Trim() == url.Trim()))
                    .Select(url => createImages(url, entiry))
                    .ToList();

                if (imageToRemove.Count > 0)
                {
                    foreach (var image in imageToRemove)
                    {
                        Console.WriteLine("imageToRemove");
                        Console.WriteLine(JsonSerializer.Serialize(image, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles }));
                    }
                }

                if (imageToAdd.Count > 0)
                {
                    foreach (var image in imageToAdd)
                    {
                        Console.WriteLine("imageToAdd");
                        Console.WriteLine(JsonSerializer.Serialize(image, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles }));
                    }
                }

                if (imageToRemove.Count > 0)
                {
                    Console.WriteLine("imageToRemove1");
                    _dbSet.RemoveRange(imageToRemove);
                    Console.WriteLine("imageToRemove2");
                }
                if (imageToAdd.Count > 0)
                {
                    Console.WriteLine("imageToAdd1");
                    await _dbSet.AddRangeAsync(imageToAdd);
                }

                return await _dataContext.SaveChangesAsync() >= 0;
            }
            catch (Exception)
            {
                return false;   
            }
        }

        protected async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
    }
