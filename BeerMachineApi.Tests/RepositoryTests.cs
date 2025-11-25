using BeerMachineApi.Services;
using Moq;
using NUnit.Framework;

using BeerMachineApi.Services;
using BeerMachineApi.Repository;
using BeerMachineApi.Services.StatusModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BeerMachineApi.Tests.Repository;

public class RepositoryTests
{

    IServiceScopeFactory? _scopeFactory;

    [SetUp]
    public void Setup()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddDbContext<MachineDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        var app = builder.Build();

        _scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        var batchHandler = new BatchHandler(_scopeFactory);
        var timeHandler = new TimeHandler(_scopeFactory);
    }

    [Category("Service")]
    [Test]
    public void DataBaseConnection()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();
            Assert.That(db.Database.CanConnect(), "could not connect to database");
        }
        catch (Exception ex)
        {
            Assert.Fail("could not connect to database");
        }
    }

    [TearDown]
    public void TearDown()
    {

    }
}