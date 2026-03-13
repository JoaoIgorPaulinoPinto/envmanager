using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.services.interfaces.project;
using static envmanager.src.data.service.dtos.MemberDtos;

namespace envmanager.src.services.usecases.project
{
    public class GetMembersFromProject : IGetMembersFromProject
    {
        private readonly IProjectRepository _projectsRepository;
        private readonly IUserRepository _userRepository;

        public GetMembersFromProject(
            IProjectRepository projectsRepository,
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _projectsRepository = projectsRepository;
        }

        public async Task<List<GetMemberResponse>?> Execute(string userId, string projId)
        {
            var project = await _projectsRepository.GetProjectById(projId);
            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            bool isAdmin = await _projectsRepository.IsAdmin(projId, userId);
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("You do not have permission to access these members.");
            }

            var members = await _projectsRepository.GetMembersFromProject(projId);
            if (members == null) return new List<GetMemberResponse>();

            var memberTasks = members.Select(async m =>
            {
                var userSchema = await _userRepository.GetSchemaById(m.Id);
                return new GetMemberResponse
                {
                    Id = m.Id,
                    Name = userSchema?.UserName ?? "Unknown User",
                    isAdmin = m.isAdmin
                };
            });

            var response = await Task.WhenAll(memberTasks);
            return response.ToList();
        }
    }
}
