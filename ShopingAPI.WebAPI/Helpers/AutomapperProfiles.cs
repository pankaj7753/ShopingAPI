using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using ShopingAPI.BusinessLayer.Helpers;
using ShopingAPI.BusinessLayer.Repository;
using ShopingAPI.DataLayer.Models;
using ShopingAPI.BusinessLayer.ViewModel;

namespace ShopingAPI.WebAPI.Helpers
{
    public class AutomapperProfiles: Profile
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        public AutomapperProfiles()
        {
            this.jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            ImageKitClass documentStorage =new ImageKitClass();
                CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Shop, ShopViewModel>()
                .ForMember(dest => dest.ShopId,opt=>opt.MapFrom(src=>src.Id))
                .ReverseMap();
            CreateMap<SubCategory, SubCategoryViewModel>().ReverseMap();
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest=>dest.ProductId,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(dest=>dest.ImageUrl,opt=>opt.MapFrom(src=> documentStorage.GetProductUri(src.ImageUrl).Result))
                .ReverseMap();
           
            CreateMap<UserAddress, UserAddressViewModel>().ReverseMap();
            CreateMap<UserAddress, AddressViewModel>().ReverseMap();
            CreateMap<Shop, AddressViewModel>().ReverseMap();
            CreateMap<OrderSummary, OrderSummaryViewModel>()
                .ForMember(dest=>dest.OrderSummaryId,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(dest=>dest.OrderDate,opt=>opt.MapFrom(src=>src.CreatedDateTime))
                .ForMember(dest=>dest.TotalAmount,opt=>opt.MapFrom(src=>(src.OrderStatusId== (int)EnumOrderStatus.PriceHiddenByShopper || src.OrderStatusId == (int)EnumOrderStatus.Cancel) ? 0:src.TotalAmount))
                .ForMember(dest=>dest.OrdersDetails,opt=>opt.MapFrom(src=>(src.OrderStatusId == (int)EnumOrderStatus.PriceHiddenByShopper || src.OrderStatusId == (int)EnumOrderStatus.Cancel)? GetOrderDetails(src.OrdersDetails):src.OrdersDetails))
                .ReverseMap();
            CreateMap<UserForListDto,User>().ReverseMap();
            CreateMap<UserForRegisterViewModel,User>().ReverseMap();
            CreateMap<BrandViewModel,Brand>().ReverseMap();
            CreateMap<FavoriteShopViewModel,FavoriteShop>().ReverseMap();
        }

        public string GetOrderDetails(string orderDetail)
        {
            var model = JsonConvert.DeserializeObject<List<VendorProductViewModel>>(orderDetail).Select(x => new VendorProductViewModel
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Description = x.Description,
                Discount = 0,
                IsOutStock = x.IsOutStock,
                MRP = 0,
                ItemCount = x.ItemCount.Value,
                Selling = 0,
                UnitType = x.UnitType,
                ProductUrl=x.ProductUrl,
                CurrencyType = x.CurrencyType
            }).ToList();
            orderDetail= Newtonsoft.Json.JsonConvert.SerializeObject(model, jsonSerializerSettings);
            return orderDetail;
        }

    }
}
