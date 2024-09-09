namespace Services;

public class VmBookingExtentionService(Context context)
{
    private readonly Context _dbService = context;

    public async Task CreateAsync(VmBookingExtention booking)
    {
        await _dbService.VmBookingExtention.AddAsync(booking);
        await _dbService.SaveChangesAsync();
    }

    public async Task<List<VmBookingExtention>> GetAllAsync()
    {
        return await _dbService.VmBookingExtention
            .Include(e => e.Owner)
            .Include(e => e.Assigned)
            .ToListAsync();
    }

    public async Task<VmBookingExtention?> GetByIdAsync(int id)
    {
        return await _dbService.VmBookingExtention
            .Where(o => o.Id == id)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .FirstOrDefaultAsync();
    }

    public async Task<List<VmBookingExtention>> GetByOwnerIdAsync(int ownerId)
    {
        return await _dbService.VmBookingExtention
            .Where(o => o.OwnerId == ownerId)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .ToListAsync();
    }

    public async Task<List<VmBookingExtention>> GetByAssignedIdAsync(int assignedToId)
    {
        return await _dbService.VmBookingExtention
            .Where(o => o.AssignedId == assignedToId)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .ToListAsync();
    }

    public async Task<VmBookingExtention?> GetByBookingId(int bookingId)
    {
        return await _dbService.VmBookingExtention.FirstOrDefaultAsync(o => o.BookingId == bookingId);
    }

    public async Task<List<VmBookingExtention>> GetListByBookingId(int bookingId)
    {
        return await _dbService.VmBookingExtention.Where(e => e.BookingId == bookingId).ToListAsync();
    }

    public async Task UpdateAsync(VmBookingExtention booking)
    {
        _dbService.VmBookingExtention.Update(booking);
        await _dbService.SaveChangesAsync();
    }

    public async Task DeleteAsync(VmBookingExtention booking)
    {
        _dbService.VmBookingExtention.Remove(booking);
        await _dbService.SaveChangesAsync();
    }
}
