using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopingAPI.DataLayer.Data;

namespace ShopingAPI.BusinessLayer.Repository
{
    public interface IAdminRepository
    {
        Task<List<dynamic>> GetUsersWithRoles();
        
    }
    public class AdminRepository : Repository, IAdminRepository
    {
        public AdminRepository(DataContext _context, IMapper _mapper) : base(_context, _mapper)
        {

        }

        public async Task<List<dynamic>> GetUsersWithRoles()
        {
            var userList = await _context.Users.OrderBy(x => x.UserName).
                        Select(user => new { 
                            Id=user.Id,
                            UserName=user.UserName,
                            FullName=user.FullName,
                            Mobile = user.Mobile,
                            Gender= user.Gender,
                            Roles=(from userRole in user.UserRoles
                                   join role in _context.Roles on userRole.RoleId equals role.Id
                                   select role.Name).ToList()

                            
                        }).ToListAsync();
            List<dynamic> dynamicList= userList.Select(x => (dynamic)x).ToList();
            return dynamicList;
        }

       
    }
}
