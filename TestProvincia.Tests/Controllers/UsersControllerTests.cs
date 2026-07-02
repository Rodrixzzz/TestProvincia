using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestProvincia.Application.DTOs;
using TestProvincia.Domain.Entities;
using TestProvincia.Domain.Enums;
using TestProvincia.Infrastructure.Data;
using TestProvinciaApi.Controllers;

namespace TestProvincia.Tests.Controllers;

public class UsersControllerTests
{
    private WebApplicationFactory<UsersController> CreateFactory(string dbName)
    {
        return new WebApplicationFactory<UsersController>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(dbName));
            });
        });
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        var factory = CreateFactory(Guid.NewGuid().ToString());
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated()
    {
        var factory = CreateFactory(Guid.NewGuid().ToString());
        var client = factory.CreateClient();
        var dto = new CreateUserDto
        {
            Name = "Carlos",
            LastName = "Garcia",
            DocumentType = "DNI",
            DocumentNumber = "33456789",
            Address = "Av. Siempre Viva 742",
            Province = "Córdoba",
            PhoneNumber = "155551234"
        };

        var response = await client.PostAsJsonAsync("/api/users", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal("Carlos", user.Name);
        Assert.Equal("33456789", user.DocumentNumber);
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        var dbName = Guid.NewGuid().ToString();
        var factory = CreateFactory(dbName);
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Users.Add(new User
            {
                Name = "Maria",
                LastName = "Lopez",
                DocumentType = DocumentType.Pasaporte,
                DocumentNumber = "37558223",
                Address = "Calle 123",
                Province = "Santa Fe",
                PhoneNumber = "342555678"
            });
            await context.SaveChangesAsync();
        }

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/users/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(result);
        Assert.Equal("Maria", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNotFound()
    {
        var factory = CreateFactory(Guid.NewGuid().ToString());
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/users/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ExistingUser_ReturnsUpdated()
    {
        var dbName = Guid.NewGuid().ToString();
        var factory = CreateFactory(dbName);
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Users.Add(new User
            {
                Name = "Original",
                LastName = "Name",
                DocumentType = DocumentType.DNI,
                DocumentNumber = "11111111"
            });
            await context.SaveChangesAsync();
        }

        var client = factory.CreateClient();
        var dto = new UpdateUserDto
        {
            Name = "Updated",
            LastName = "Name",
            DocumentType = "Pasaporte",
            DocumentNumber = "99999999",
            Address = "Calle falsa 123",
            Province = "Mendoza",
            PhoneNumber = "261555000"
        };

        var response = await client.PutAsJsonAsync("/api/users/1", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
        Assert.Equal(DocumentType.Pasaporte, result.DocumentType);
    }

    [Fact]
    public async Task UpdateUser_NonExistingUser_ReturnsNotFound()
    {
        var factory = CreateFactory(Guid.NewGuid().ToString());
        var client = factory.CreateClient();
        var dto = new UpdateUserDto
        {
            Name = "Nobody",
            LastName = "Nowhere",
            DocumentType = "DNI",
            DocumentNumber = "00000000"
        };

        var response = await client.PutAsJsonAsync("/api/users/999", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ExistingUser_ReturnsNoContent()
    {
        var dbName = Guid.NewGuid().ToString();
        var factory = CreateFactory(dbName);
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Users.Add(new User
            {
                Name = "ToDelete",
                LastName = "User",
                DocumentType = DocumentType.DNI,
                DocumentNumber = "22222222"
            });
            await context.SaveChangesAsync();
        }

        var client = factory.CreateClient();
        var response = await client.DeleteAsync("/api/users/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_NonExistingUser_ReturnsNotFound()
    {
        var factory = CreateFactory(Guid.NewGuid().ToString());
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/users/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
