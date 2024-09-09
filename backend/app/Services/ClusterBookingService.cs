namespace Services;

public class ClusterBookingService(Context context)
{
    private readonly Context _dbService = context;

    public async Task CreateAsync(ClusterBooking booking)
    {
        await _dbService.ClusterBookings.AddAsync(booking);
        await _dbService.SaveChangesAsync();
    }

    public async Task<List<ClusterBooking>> GetAllAsync()
    {
        return await _dbService.ClusterBookings
            .Include(b => b.VCenters)
            .ThenInclude(vc => vc.EsxiHosts)
            .Include(b => b.Owner)
            .ToListAsync();
    }

    public async Task<ClusterBooking?> GetByIdAsync(int? id)
    {
        return await _dbService.ClusterBookings.Where(o => o.Id == id)
            .Include(b => b.VCenters)
            .ThenInclude(vc => vc.EsxiHosts)
            .Include(b => b.Owner)
            .FirstOrDefaultAsync();
    }

    public async Task<ClusterBooking?> GetActiveByVcenterIdAsync(int id)
    {
        VCenter? vCenter = await _dbService.VCenters.FirstOrDefaultAsync(o => o.Id == id);
        if (vCenter == null || vCenter.BookingId == null)
        {
            return null;
        }

        return await GetByIdAsync(vCenter.BookingId);
    }

    public async Task<List<ClusterBooking>> GetByOwnerAsync(int ownerId)
    {
        return await _dbService.ClusterBookings.Where(o => o.OwnerId == ownerId)
            .Include(b => b.VCenters)
            .ThenInclude(vc => vc.EsxiHosts)
            .Include(b => b.Owner)
            .ToListAsync();
    }

    public async Task UpdateAsync(ClusterBooking booking)
    {
        _dbService.ClusterBookings.Update(booking);
        await _dbService.SaveChangesAsync();
    }

    public async Task DeleteAsync(ClusterBooking booking)
    {
        _dbService.ClusterBookings.Remove(booking);
        await _dbService.SaveChangesAsync();
    }
}
