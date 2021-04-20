using AutoMapper;
using Microsoft.Extensions.Options;
using ShopingAPI.BusinessLayer.Helpers;
using ShopingAPI.DataLayer.Data;

namespace ShopingAPI.BusinessLayer.Repository
{
    public interface ISingletonRepository
    {
        IProductRepository productRepository { get; }
        IVendorRepository   vendorRepository { get; }
        IUserRepository   userRepository { get; }
        IOrderRepository orderRepository { get; }
        IAuthRepository authRepository { get; }
        IAdminRepository adminRepository { get; }

    }
    public class SingletonRepository : ISingletonRepository
    {
        private readonly DataContext _context;
        protected readonly IMapper _mapper;
        private readonly IOptions<AppSettings> appSettings;

        public SingletonRepository(DataContext context, IMapper mapper, IOptions<AppSettings> app)
        {
            _context = context;
            _mapper = mapper;
            appSettings = app;

        }

        private IAdminRepository admin;
        public IAdminRepository adminRepository
        {
            get
            {
                if (admin == null)
                {
                    admin = new AdminRepository(_context,_mapper);
                }
                return admin;
            }
        }
        #region Old code

        private IAuthRepository auth;
        public IAuthRepository authRepository
        {
            get
            {
                if (auth == null)
                {
                    auth = new AuthRepository(_context, _mapper);
                }
                return auth;
            }
        }

        private IOrderRepository order;
        public IOrderRepository orderRepository
        {
            get
            {
                if (order == null)
                {
                    order = new OrderRepository(_context, _mapper);
                }
                return order;
            }
        }
        private IUserRepository user;
        public IUserRepository userRepository
        {
            get
            {
                if (user == null)
                {
                    user = new UserRepository(_context, _mapper);
                }
                return user;
            }
        }


        private IVendorRepository vendor;
        public IVendorRepository vendorRepository
        {
            get
            {
                if (vendor == null)
                {
                    vendor = new VendorRepository(_context, _mapper,appSettings);
                }
                return vendor;
            }
        }

        private IProductRepository product;
        public IProductRepository productRepository
        {
            get
            {
                if (product == null)
                {
                    product = new ProductRepository(_context, _mapper);
                }
                return product;
            }
        }

        // => throw new NotImplementedException();
        #endregion
    }
}
