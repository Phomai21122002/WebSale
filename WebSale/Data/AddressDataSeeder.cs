using Microsoft.EntityFrameworkCore;
using System.IO.Pipelines;
using WebSale.Models;
using WebSale.Services;

namespace WebSale.Data
{
    public class AddressDataSeeder
    {
        private readonly DataContext _context;

        public AddressDataSeeder(DataContext context)
        {
            _context = context;
        }

        public async void SeedProvinces(string folderPath)
        {
            if (!_context.Provinces.Any())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var provinces = FileReader.ReadJsonFiles<Provinces>(folderPath);
                    _context.Provinces.AddRange(provinces);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async void SeedDistricts(string folderPath)
        {
            if (!_context.Districts.Any())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var districts = FileReader.ReadJsonFiles<District>(folderPath);
                    _context.Districts.AddRange(districts);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async void SeedWards(string folderPath)
        {
            if (!_context.Wards.Any())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var wards = FileReader.ReadJsonFiles<Ward>(folderPath);
                    _context.Wards.AddRange(wards);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
