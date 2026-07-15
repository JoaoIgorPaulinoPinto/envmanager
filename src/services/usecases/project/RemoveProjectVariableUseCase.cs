using System;

public class RemoveProjectVariableUseCase : 
{
    private readonly IProjectRepository _projectRepository;

    public RemoveProjectVariableUseCase(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

}
    