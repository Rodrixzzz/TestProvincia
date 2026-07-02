using TestProvincia.Domain.Entities;

namespace TestProvincia.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
}
