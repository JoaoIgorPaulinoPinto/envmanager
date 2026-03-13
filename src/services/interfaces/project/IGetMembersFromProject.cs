using envmanager.src.data.service.schemes;
using static envmanager.src.data.service.dtos.MemberDtos;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.interfaces.project
{
    public interface IGetMembersFromProject
    {
        public Task<List<GetMemberResponse>?> Execute(string userId, string projId);
    }
}
