using envmanager.src.data.schemes;
using envmanager.src.infra.dtos;

namespace envmanager.src.infra.mappers
{
    public class UserMapping
    {
        public UsersDtos.GetUsersResponse SchemaToDTO(User user) {
            return new UsersDtos.GetUsersResponse() { 
                id = user.Id,
                email = user.Email,
                user_name = user.UserName, 
                password = user.Password
            };
        }

    }
}
