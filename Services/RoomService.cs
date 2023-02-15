using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using HogwartsPotions.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Services;

public class RoomService : IRoomService
{
    private readonly HogwartsContext _context;

    public RoomService(HogwartsContext context)
    {
        _context = context;
    }

    public async Task AddRoom(Room room)
    {
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
    }

    public Task<Room> GetRoom(long roomId)
    {
        var room = _context.Rooms
            .Include(r => r.Residents)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ID == roomId);
        return room;
    }

    public Task<List<Room>> GetAllRooms()
    {
        return _context.Rooms
            .Include(r => r.Residents)
            .ToListAsync();
    }

    public async Task UpdateRoom(Room room)
    {
        if (_context.Entry(room).State == EntityState.Detached)
        {
            _context.Entry(room).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoom(Room room)
    {
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }

    public Task<List<Room>> GetRoomsForRatOwners()
    {
        return _context.Rooms
            .Include(r => r.Residents)
            .Where(r => r.Residents
                .All(s => s.PetType != PetType.Cat && s.PetType != PetType.Owl)
            )
            .ToListAsync();
    }
}