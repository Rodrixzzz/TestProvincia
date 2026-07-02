using Microsoft.EntityFrameworkCore;
using TestProvincia.Domain.Entities;
using TestProvincia.Domain.Interfaces;
using TestProvincia.Infrastructure.Data;

namespace TestProvincia.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.Include(u => u.DocumentType).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.Include(u => u.DocumentType).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _context.Entry(user).Reference(u => u.DocumentType).LoadAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        await _context.Entry(user).Reference(u => u.DocumentType).LoadAsync();
        return user;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is not null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetByDocumentNumberAsync(string number, string type)
    {
        var docType = await _context.DocumentTypes.FirstOrDefaultAsync(d => d.Desc == type);
        if (docType is null) return null;

        return await _context.Users.Include(u => u.DocumentType).FirstOrDefaultAsync(
            x => x.DocumentNumber == number && x.DocumentTypeId == docType.Id);
    }

    public async Task<DocumentType?> GetDocumentTypeByDescAsync(string desc)
    {
        return await _context.DocumentTypes.FirstOrDefaultAsync(d => d.Desc == desc);
    }
}
