using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.project;
using static GlobalExceptionHandler;

namespace envmanager.src.services.usecases.project
{
    public class GetProjectsUseCase : IGetProjectsUseCase
    {
        private readonly IProjectRepository _projectsRepository;
        private readonly IUserRepository _userRepository;
        private readonly SecurityService _securityService;

        public GetProjectsUseCase(
            IProjectRepository projectsRepository,
            IUserRepository userRepository,
            SecurityService securityService)
        {
            _projectsRepository = projectsRepository;
            _userRepository = userRepository;
            _securityService = securityService;
        }

        public async Task<List<ProjectDtos.GetProjectsResponse>> Execute(string userId)
        {
            ValidateUserId(userId);

            var projects = await _projectsRepository.GetProjectsByUser(userId);
            if (projects.Count == 0)
                throw new KeyNotFoundException("No projects found in the database.");

            return projects.Select(p => new ProjectDtos.GetProjectsResponse
            {
                id = p.Id,
                name = p.ProjectName,
                description = p.Description,
                access_link = p.AccesLink,
                isOwner = p.UserId == userId,
                need_password = !string.IsNullOrEmpty(p.Password)
            }).ToList();
        }

        public async Task<ProjectDtos.GetProjectByIdResponse> Execute(string userId, string projId)
        {
            ValidateUserAndProject(userId, projId);

            var project = await GetAccessibleProject(userId, projId);
            if (!string.IsNullOrEmpty(project.Password))
                throw new BusinessException("Password is required to access this project");

            return await MapProjectDetails(project, userId);
        }

        public async Task<ProjectDtos.GetProjectByIdResponse> Execute(string userId, string projId, string password)
        {
            ValidateUserAndProject(userId, projId);

            var project = await GetAccessibleProject(userId, projId);

            if (!string.IsNullOrEmpty(project.Password))
            {
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentNullException("Password is required to access this project");

                var isPasswordValid = _securityService.VerifyPassword(project.Password, password);
                if (!isPasswordValid)
                    throw new BusinessException("Access denied. Invalid password.");
            }

            return await MapProjectDetails(project, userId);
        }

        private static void ValidateUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("The provided User ID is invalid.");
        }

        private static void ValidateUserAndProject(string userId, string projId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(projId))
                throw new ArgumentException("The provided User ID is invalid.");
        }

        private async Task<Project> GetAccessibleProject(string userId, string projId)
        {
            var project = await _projectsRepository.GetProjectById(projId);
            if (project == null)
                throw new KeyNotFoundException("Project not found.");

            var isOwner = project.UserId == userId;
            var isMember = project.Members.Any(m => m.Id == userId);
            if (!isOwner && !isMember)
                throw new UnauthorizedAccessException("You do not have access to this project.");

            return project;
        }

        private async Task<ProjectDtos.GetProjectByIdResponse> MapProjectDetails(Project project, string userId)
        {
            var members = new List<MemberDtos.GetMemberResponse>();
            foreach (var member in project.Members)
            {
                var user = await _userRepository.GetSchemaById(member.Id);
                members.Add(new MemberDtos.GetMemberResponse
                {
                    Name = user?.UserName ?? "Unknown User",
                    isAdmin = member.isAdmin
                });
            }

            return new ProjectDtos.GetProjectByIdResponse
            {
                id = project.Id,
                name = project.ProjectName,
                description = project.Description,
                isOwner = project.UserId == userId,
                access_link = project.AccesLink,
                members = members,
                variables = project.Variables.Select(v => new KeyDTos.GetVariableResponse
                {
                    id = v.Id ?? string.Empty,
                    variable = v.Variable,
                    Value = v.Value
                }).ToList()
            };
        }
    }
}
