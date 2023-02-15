using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;

namespace HogwartsPotions.Services.Interface;

public interface IRoomService
{
    Task AddRoom(Room room);
    Task<Room> GetRoom(long roomId);

    Task<List<Room>> GetAllRooms();

    Task UpdateRoom(Room room);
    Task DeleteRoom(Room room);

    Task<List<Room>> GetRoomsForRatOwners();
}