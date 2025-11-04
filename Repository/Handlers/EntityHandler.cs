using BeerMachineApi.Repository;
using BeerMachineApi.Services.DTOs;

public abstract class EntityHandler
{
    protected IServiceScopeFactory _scopeFactory;

    public EntityHandler( IServiceScopeFactory scopeFactory )
    {
        _scopeFactory = scopeFactory;
    }
}