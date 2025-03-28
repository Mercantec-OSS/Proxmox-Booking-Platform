﻿namespace Repositories;

public class UserRepository(Context context)
{
    private readonly Context _dbService = context;

    public async Task CreateAsync(User user)
    {
        _ = await _dbService.Users.AddAsync(user);
        _ = await _dbService.SaveChangesAsync();
    }

    public async Task<User?> GetAsync(int id)
    {
        return await _dbService.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<User?> GetAsync(string email)
    {
        return await _dbService.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<List<User>> GetAllAsync()
    {
        return await _dbService.Users.ToListAsync();
    }

    public async Task<int> GetUserCountAsync()
    {
        return await _dbService.Users.CountAsync();
    }

    public async Task<User?> GetUserByFullNameAsync(string name)
    {
        return await _dbService.Users.FirstOrDefaultAsync(u => 
            u.Name + " " + u.Surname == name);
    }

    public async Task<User?> GetAsync(string email, string password)
    {
        return await _dbService.Users.FirstOrDefaultAsync(u =>
            u.Password == password && u.Email == email
        );
    }

    public async Task UpdateAsync(User user)
    {
        _ = _dbService.Users.Update(user);
        _ = await _dbService.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _ = _dbService.Users.Remove(user);
        _ = await _dbService.SaveChangesAsync();
    }
}
