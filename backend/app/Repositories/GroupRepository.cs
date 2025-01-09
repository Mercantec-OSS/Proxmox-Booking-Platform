namespace Repositories;

public class GroupRepository(Context context)
{
    private readonly Context _dbService = context;

    public async Task CreateAsync(Group obj)
    {
        await _dbService.studentGroups.AddAsync(obj);
        await _dbService.SaveChangesAsync();
    }

    public async Task<List<Group>> GetAsync()
    {
        return await _dbService.studentGroups
            .Include(g => g.Members)
            .ToListAsync();
    }

    public async Task<Group?> GetByIDAsync(int groupId)
    {
        return await _dbService.studentGroups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(o => o.Id == groupId);
    }

    public async Task<Group?> GetByClassAsync(string className)
    {
        return await _dbService.studentGroups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(o => o.Name == className);
    }

    public async Task DeleteAsync(Group group)
    {
        _dbService.studentGroups.Remove(group);
        await _dbService.SaveChangesAsync();
    }
}
