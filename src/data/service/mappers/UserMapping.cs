using envmanager.src.data.service.dtos;
using envmanager.src.data.service.schemes;

namespace envmanager.src.data.service.mappers
{
    public class UserMapping
    {
        public UsersDtos.GetUsersResponse SchemaToDTO(User user) {
            return new UsersDtos.GetUsersResponse() { 
                email = user.Email,
                user_name = user.UserName, 
            };
        }

    }
}
