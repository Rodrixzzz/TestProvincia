using FluentValidation;
using TestProvincia.Application.DTOs;
using TestProvincia.Application.Interfaces;
using TestProvincia.Application.Mappers;
using TestProvincia.Application.Validations;
using TestProvincia.Domain.Entities;
using TestProvincia.Domain.Interfaces;
using TestProvincia.Shared.Constants;
using TestProvincia.Shared.Exceptions;

namespace TestProvincia.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private IValidator<CreateUserDto> _validator;
    public UserService(IUserRepository repository, IValidator<CreateUserDto> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(MapsHelper.MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user is null ? null : MapsHelper.MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        _validator.ValidateAndThrow(dto);

        var existing = await _repository.GetByDocumentNumberAsync(dto.DocumentNumber, dto.DocumentType);
        if (existing != null)
            throw new DuplicatedException(dto.DocumentType + " " + dto.DocumentNumber + Messages.Error.DuplicatedUser);

        var docType = await _repository.GetDocumentTypeByDescAsync(dto.DocumentType);

        var user = new User
        {
            Name = dto.Name,
            LastName = dto.LastName,
            DocumentTypeId = docType.Id,
            DocumentNumber = dto.DocumentNumber,
            Address = dto.Address,
            Province = dto.Province,
            PhoneNumber = dto.PhoneNumber
        };

        var created = await _repository.AddAsync(user);
        return MapsHelper.MapToDto(created);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new NotFoundException(id + Messages.Error.NotExistUser);

        _validator.ValidateAndThrow(dto);
        var docType = existing.DocumentType;

        if (existing.DocumentNumber != dto.DocumentNumber || existing.DocumentType.Desc != dto.DocumentType)
        {
            var documentExist = await _repository.GetByDocumentNumberAsync(dto.DocumentNumber, dto.DocumentType);
            if (documentExist != null)
            {
                throw new DuplicatedException(dto.DocumentType + " " + dto.DocumentNumber + Messages.Error.DuplicatedUser);
            }
            docType = await _repository.GetDocumentTypeByDescAsync(dto.DocumentType);
        }


        existing.Name = dto.Name;
        existing.LastName = dto.LastName;
        existing.DocumentTypeId = docType.Id;
        existing.DocumentNumber = dto.DocumentNumber;
        existing.Address = dto.Address;
        existing.Province = dto.Province;
        existing.PhoneNumber = dto.PhoneNumber;

        var updated = await _repository.UpdateAsync(existing);
        return MapsHelper.MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new NotFoundException(id + Messages.Error.NotExistUser);

        await _repository.DeleteAsync(id);
        return true;
    }

    
}
