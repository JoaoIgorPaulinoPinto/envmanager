using envmanager.src.services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace envmanager.src.app.controllers
{
    [ApiController]
    public sealed class UserController : ControllerBase
    {
        private readonly IGetUsersUseCase _getUsersUseCase;
        public UserController(IGetUsersUseCase getUsersUseCase) { 
        
            _getUsersUseCase = getUsersUseCase;
        }

    }
}
