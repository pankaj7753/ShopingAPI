using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.BusinessLayer.Helpers
{
    public interface ISessionUser
    {
        public Task<string> GetUserOrdersTable(string userId);
    }

    public  class SessionUser: ShopingAPI.BusinessLayer.Repository.Repository, ISessionUser
    {
        public SessionUser(DataContext _context, IMapper _mapper) : base(_context, _mapper)
        {
        }
        public async Task<string> GetUserOrdersTable(string userId)
        {
            User user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                return user.TableName;
            }
            return null;
        }
    }
}
