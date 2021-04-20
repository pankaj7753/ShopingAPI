using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.BusinessLayer.Helpers;
using ShopingAPI.BusinessLayer.Repository;

namespace ShopingAPI.WebAPI.WebAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommanCtorController : ControllerBase
    {
        protected readonly DataContext _context;
        protected readonly IMapper _mapper;
        protected readonly ISingletonRepository _singleton;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<CommanCtorController> _logger;
        public CommanCtorController(DataContext context, IMapper mapper, ILogger<CommanCtorController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public CommanCtorController(DataContext context, IConfiguration configuration, ISingletonRepository singleton, ILogger<CommanCtorController> logger)
        {
            _context = context;
            _configuration = configuration;
            _singleton = singleton;
            _logger = logger;
        }
        public CommanCtorController(ISingletonRepository singleton, ILogger<CommanCtorController> logger)
        {
            _singleton = singleton;
            _logger = logger;
        }

        public CommanCtorController(ISingletonRepository singleton, ILogger<CommanCtorController> logger, IMapper mapper)
        {
            _singleton = singleton;
            _logger = logger;
            _mapper = mapper;

        }
    }
}