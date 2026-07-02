using Moq;
using TestProvincia.Application.DTOs;
using TestProvincia.Application.Services;
using TestProvincia.Domain.Entities;
using TestProvincia.Domain.Interfaces;

namespace TestProvincia.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _service = new UserService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "John", LastName = "Doe", DocumentType = DocumentType.DNI, DocumentNumber = "12345678" },
            new() { Id = 2, Name = "Jane", LastName = "Smith", DocumentType = DocumentType.Pasaporte, DocumentNumber = "AB123456" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        var user = new User { Id = 1, Name = "John", LastName = "Doe", DocumentType = DocumentType.DNI, DocumentNumber = "12345678" };
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
            Name = "Alice",
            LastName = "Johnson",
            DocumentType = DocumentType.DNI,
            DocumentNumber = "87654321",
            Address = "123 Main St",
            Province = "Buenos Aires",
            PhoneNumber = "123456789"
        };

        var createdUser = new User
        {
            Id = 1,
            Name = dto.Name,
            LastName = dto.LastName,
            DocumentType = dto.DocumentType,
            DocumentNumber = dto.DocumentNumber,
            Address = dto.Address,
            Province = dto.Province,
            PhoneNumber = dto.PhoneNumber
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("87654321", result.DocumentNumber);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_ReturnsUpdatedUser()
    {
        var existingUser = new User { Id = 1, Name = "Old", LastName = "Name", DocumentType = DocumentType.DNI, DocumentNumber = "00000000" };
        var dto = new UpdateUserDto
        {
            Name = "New",
            LastName = "Name",
            DocumentType = DocumentType.Pasaporte,
            DocumentNumber = "ZZ999999",
            Address = "456 Oak Ave",
            Province = "CABA",
            PhoneNumber = "987654321"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("New", result.Name);
        Assert.Equal(DocumentType.Pasaporte, result.DocumentType);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingUser_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _service.UpdateAsync(99, new UpdateUserDto());

        Assert.Null(result);
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
    public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }
}
