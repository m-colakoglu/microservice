using AutoMapper;
using Discount.Gprc.Entities;
using Discount.Gprc.Protos;

namespace Discount.Gprc.Mapper
{
    public class DiscountProfile:Profile
    {
        public DiscountProfile()
        {
            CreateMap<Coupon, CouponModel>().ReverseMap();
        }
    }
}
