namespace Services;

public class VCenterService(Context context)
{
    private readonly Context _dbService = context;

    public async Task CreateAsync(VCenter obj)
    {
        await _dbService.VCenters.AddAsync(obj);
        await _dbService.SaveChangesAsync();
    }

    public async Task<VCenter?> GetByIdAsync(int id)
    {
        return await _dbService.VCenters
            .Include(v => v.EsxiHosts)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<VCenter?> GetByIpAsync(string ip)
    {
        return await _dbService.VCenters.FirstOrDefaultAsync(o => o.Ip == ip.Trim());
    }

    public async Task<List<VCenter>> GetAllAsync()
    {
        return await _dbService.VCenters.Include(v => v.EsxiHosts).ToListAsync();
    }

    public async Task<bool> UpdateAsync(VCenter obj)
    {
        obj.UpdatedAt = DateTime.UtcNow;
        _dbService.VCenters.Update(obj);
        await _dbService.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(VCenter obj)
    {
        _dbService.VCenters.Remove(obj);
        await _dbService.SaveChangesAsync();

        return true;
    }

    public async Task<List<VCenter>> GetAvailableAsync()
    {
        List<VCenter> vCenters = await GetAllAsync();
        return vCenters.Where(v => v.BookingId == null).ToList();
    }

    public async Task<List<VCenter>> GetUsedAsync()
    {
        List<VCenter> vCenters = await GetAllAsync();
        return vCenters.Where(v => v.BookingId != null).ToList();
    }

    public async Task<List<VCenter>> GetByBookingIdAsync(int bookingId)
    {
        return await _dbService.VCenters.Where(v => v.BookingId == bookingId).ToListAsync();
    }
}
