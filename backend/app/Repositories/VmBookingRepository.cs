namespace Repositories;

public class VmBookingRepository(Context context)
{
    private readonly Context _dbService = context;

    public async Task CreateAsync(VmBooking booking)
    {
        _ = await _dbService.VmBookings.AddAsync(booking);
        _ = await _dbService.SaveChangesAsync();
    }

    public async Task<List<VmBooking>> GetAllAsync()
    {
        return await _dbService.VmBookings
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Owner)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Assigned)
            .ToListAsync();
    }

    public async Task<VmBooking?> GetByIdAsync(int id)
    {
        return await _dbService.VmBookings
        .Where(o => o.Id == id)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Owner)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Assigned)
            .FirstOrDefaultAsync();
    }

    public async Task<VmBooking?> GetByNameAsync(string name)
    {
        return await _dbService.VmBookings
        .Where(o => o.Name == name)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Owner)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Assigned)
            .FirstOrDefaultAsync();
    }

    public async Task<List<VmBooking>> GetByOnwerIdAsync(int ownerId)
    {
        return await _dbService.VmBookings
            .Where(o => o.OwnerId == ownerId)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Owner)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Assigned)
            .ToListAsync();
    }

    public async Task<List<VmBooking>> GetByAssignedToIdAsync(int assignedToId)
    {
        return await _dbService.VmBookings
        .Where(o => o.AssignedId == assignedToId)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Owner)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Assigned)
            .ToListAsync();
    }

    public async Task UpdateAsync(VmBooking booking)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        _ = _dbService.VmBookings.Update(booking);
        _ = await _dbService.SaveChangesAsync();
    }

    public async Task DeleteAsync(VmBooking booking)
    {
        _ = _dbService.VmBookings.Remove(booking);
        _ = await _dbService.SaveChangesAsync();
    }

    public async Task<List<VmBooking>> GetExpiredAsync()
    {
        return await _dbService.VmBookings
            .Where(b => b.ExpiredAt < DateTime.UtcNow)
            .Include(b => b.Owner)
            .Include(b => b.Assigned)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Owner)
            .Include(b => b.Extentions)
            .ThenInclude(e => e.Assigned)
            .ToListAsync();
    }
}
