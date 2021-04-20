using AutoMapper;
using ShopingAPI.DataLayer.Data;

namespace ShopingAPI.BusinessLayer.Repository
{
    public class Repository
    {
        protected readonly DataContext _context;
        protected readonly IMapper _mapper;
        public Repository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
