 using Backend.Domain.Entities;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

    public UserController(UserService userService)
    {
        this.userService = userService;
    }
        [Authorize]
        [HttpGet("{id}", Name = "GetUserById")]
        public User GetUserById(string id) => userService.GetUserById(id);
    }
}
