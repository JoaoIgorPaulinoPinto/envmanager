using System.Globalization;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.interfaces
{
    public interface IUpdateProjectVariables
    {
        public Task<bool> Execute(UpdateVariablesRequest updateVariablesRequest, string userId);
    }
}
