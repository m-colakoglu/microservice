using AutoMapper;
using Discount.Gprc.Protos;
using Discount.Gprc.Repositories;
using Grpc.Core;

namespace Discount.Gprc.Services
{
    public class DiscountService:DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;
        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger,IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon= await _repository.GetDiscount(request.ProductName);

            if (coupon == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found"));

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }
    }
}
