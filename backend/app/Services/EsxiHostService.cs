namespace Services;

public class EsxiHostService(Context context)
{
    private readonly Context _dbService = context;

    public async Task CreateAsync(EsxiHost obj)
    {
        await _dbService.EsxiHosts.AddAsync(obj);
        await _dbService.SaveChangesAsync();
    }

    public async Task<EsxiHost?> GetByIdAsync(int id)
    {
        return await _dbService.EsxiHosts
            .Include(h => h.VCenter)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<EsxiHost?> GetByIpAsync(string ip)
    {
        return await _dbService.EsxiHosts.FirstOrDefaultAsync(o => o.Ip == ip);
    }

    public async Task<List<EsxiHost>> GetAllAsync()
    {
        return await _dbService.EsxiHosts
            .Include(h => h.VCenter)
            .ToListAsync();
    }

    public async Task<List<EsxiHost>> GetByVcenterAsync(int vcenterId)
    {
        return await _dbService.EsxiHosts
            .Where(o => o.VCenterId == vcenterId)
            .Include(h => h.VCenter)
            .ToListAsync();
    }

    public async Task<int> UpdateAsync(EsxiHost obj)
    {
        obj.UpdatedAt = DateTime.UtcNow;
        _dbService.EsxiHosts.Update(obj);
        await _dbService.SaveChangesAsync();
        return obj.Id;
    }
    public async Task<int> DeleteAsync(EsxiHost obj)
    {
        _dbService.EsxiHosts.Remove(obj);
        await _dbService.SaveChangesAsync();
        return obj.Id;
    }
}
