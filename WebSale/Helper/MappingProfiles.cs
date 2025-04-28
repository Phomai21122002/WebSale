using AutoMapper;
using WebSale.Dto.Addresses;
using WebSale.Dto.Auth;
using WebSale.Dto.Categories;
using WebSale.Dto.ProductDetails;
using WebSale.Dto.Roles;
using WebSale.Dto.Users;
using WebSale.Models;

namespace WebSale.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {
            CreateMap<RegisterDto, User>();
            CreateMap<RoleDto, Role>();
            CreateMap<RoleUpdateDto, Role>();
            CreateMap<UserBaseDto, User>();
            CreateMap<CategoryBase, Category>();
            CreateMap<CategoryDto, Category>();
            CreateMap<ProductDetailDto, ProductDetail>();
            CreateMap<ProductDetailDto, Product>();
            CreateMap<Product, ProductDetail>();
            CreateMap<AddressCreateDto, Address>();
        }
    }
}
