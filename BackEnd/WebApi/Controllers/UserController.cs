using Backend.Domain.Entities;
using Backend.Domain.Services;
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

        private static byte[] ImageToBinary(string imagePath)
        {
            FileStream fS = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            byte[] b = new byte[fS.Length];
            fS.Read(b, 0, (int)fS.Length);
            fS.Close();
            return b;
        }

        [HttpGet("recommendations/{id:int}", Name = "GetRecommendations")]
        public List<User> GetRecomenndationsForUser(int id) => userService.GetRecommendationsForUser(id);

        [HttpGet("{id:int}", Name = "GetUserById")]
        public User GetUserById(int id) => userService.GetUserById(id);
    }
}
