using Application.Interface.User;
using Domain.Entities.Invoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceManagementSystemAPI.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserService _UserService;

        public UserController(IUserService UserService)
        {
            _UserService = UserService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserInfoByUserID(string UserID)
        {
            var USerInfo = await _UserService.GetUserInfoByUserID(UserID);
            return Ok(USerInfo);
        }
    }
}
