using FluentValidation;
using Moq;
using TestProvincia.Application.DTOs;
using TestProvincia.Application.Services;
using TestProvincia.Domain.Entities;
using TestProvincia.Domain.Interfaces;
using TestProvincia.Shared.Exceptions;

namespace TestProvincia.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IValidator<CreateUserDto>> _validatorMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IValidator<CreateUserDto>>();
        _service = new UserService(_repositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Carlos", LastName = "Garcia", DocumentTypeId = 1, DocumentType = new DocumentType { Id = 1, Desc = "DNI" }, DocumentNumber = "12345678" },
            new() { Id = 2, Name = "Gonzalo", LastName = "Gomez", DocumentTypeId = 2, DocumentType = new DocumentType { Id = 2, Desc = "Pasaporte" }, DocumentNumber = "35456989" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        var user = new User { Id = 1, Name = "Carlos", LastName = "Garcia", DocumentTypeId = 1, DocumentType = new DocumentType { Id = 1, Desc = "DNI" }, DocumentNumber = "12345678" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Carlos", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingUser_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedUser()
    {
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

        var createdUser = new User
        {
            Id = 1,
            Name = dto.Name,
            LastName = dto.LastName,
            DocumentTypeId = 1,
            DocumentType = new DocumentType { Id = 1, Desc = "DNI" },
            DocumentNumber = dto.DocumentNumber,
            Address = dto.Address,
            Province = dto.Province,
            PhoneNumber = dto.PhoneNumber
        };

        _repositoryMock.Setup(r => r.GetDocumentTypeByDescAsync("DNI"))
            .ReturnsAsync(new DocumentType { Id = 1, Desc = "DNI", Active = true });
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Carlos", result.Name);
        Assert.Equal("33456789", result.DocumentNumber);
    }

    [Fact]
    public async Task CreateAsync_DuplicateDocument_ThrowsDuplicatedException()
    {
        var dto = new CreateUserDto
        {
            Name = "Otro",
            LastName = "User",
            DocumentType = "DNI",
            DocumentNumber = "33456789",
            Address = "Calle 123",
            Province = "CABA",
            PhoneNumber = "123456789"
        };

        _repositoryMock.Setup(r => r.GetByDocumentNumberAsync("33456789", "DNI"))
            .ReturnsAsync(new User { Id = 99 });

        await Assert.ThrowsAsync<DuplicatedException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_ReturnsUpdatedUser()
    {
        var existingUser = new User { Id = 1, Name = "Viejo", LastName = "Name", DocumentTypeId = 1, DocumentType = new DocumentType { Id = 1, Desc = "DNI" }, DocumentNumber = "00000000" };
        var dto = new UpdateUserDto
        {
            Name = "Carlos",
            LastName = "Garcia",
            DocumentType = "DNI",
            DocumentNumber = "33456789",
            Address = "Av. Siempre Viva 742",
            Province = "Córdoba",
            PhoneNumber = "155551234"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);
        _repositoryMock.Setup(r => r.GetDocumentTypeByDescAsync("DNI"))
            .ReturnsAsync(new DocumentType { Id = 1, Desc = "DNI", Active = true });
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Carlos", result.Name);
        Assert.Equal("DNI", result.DocumentType);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateDocument_ThrowsDuplicatedException()
    {
        var existingUser = new User { Id = 1, Name = "Viejo", LastName = "Name", DocumentTypeId = 1, DocumentType = new DocumentType { Id = 1, Desc = "DNI" }, DocumentNumber = "00000000" };
        var dto = new UpdateUserDto
        {
            Name = "Otro",
            LastName = "User",
            DocumentType = "DNI",
            DocumentNumber = "33456789",
            Address = "Calle 123",
            Province = "CABA",
            PhoneNumber = "123456789"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);
        _repositoryMock.Setup(r => r.GetByDocumentNumberAsync("33456789", "DNI"))
            .ReturnsAsync(new User { Id = 99 });

        await Assert.ThrowsAsync<DuplicatedException>(() => _service.UpdateAsync(1, dto));
    }

    [Fact]
    public async Task UpdateAsync_NonExistingUser_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(99, new UpdateUserDto()));
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_ReturnsTrue()
    {
        var user = new User { Id = 1, Name = "Test", LastName = "User", DocumentTypeId = 1, DocumentType = new DocumentType { Id = 1, Desc = "DNI" }, DocumentNumber = "12345678" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));
    }
}
