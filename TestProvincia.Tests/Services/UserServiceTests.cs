using FluentValidation;
using Moq;
using TestProvincia.Application.DTOs;
using TestProvincia.Application.Services;
using TestProvincia.Domain.Entities;
using TestProvincia.Domain.Enums;
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
            new() { Id = 1, Name = "Carlos", LastName = "Garcia", DocumentType = DocumentType.DNI, DocumentNumber = "Carlos" },
            new() { Id = 2, Name = "Gonzalo", LastName = "Gomez", DocumentType = DocumentType.Pasaporte, DocumentNumber = "35456989" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        var user = new User { Id = 1, Name = "Carlos", LastName = "Garcia", DocumentType = DocumentType.DNI, DocumentNumber = "Carlos" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingUser_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
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
            DocumentType = DocumentType.DNI,
            DocumentNumber = dto.DocumentNumber,
            Address = dto.Address,
            Province = dto.Province,
            PhoneNumber = dto.PhoneNumber
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Carlos", result.Name);
        Assert.Equal("33456789", result.DocumentNumber);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_ReturnsUpdatedUser()
    {
        var existingUser = new User { Id = 1, Name = "Viejo", LastName = "Name", DocumentType = DocumentType.DNI, DocumentNumber = "00000000" };
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
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("New", result.Name);
        Assert.Equal(DocumentType.Pasaporte, result.DocumentType);
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
        var user = new User { Id = 1, Name = "Test", LastName = "User", DocumentType = DocumentType.DNI, DocumentNumber = "12345678" };
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
