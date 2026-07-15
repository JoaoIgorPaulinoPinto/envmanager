
using static envmanager.src.data.service.dtos.ProjectDtos;
namespace envmanager.src.services.interfaces.project
{ 
    public interface IRemoveProjectVariableUseCase
    {
        public Task<bool> Execute(RemoveVariableRequest dto, int userId);
    }
}