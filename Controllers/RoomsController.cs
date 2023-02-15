using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data;
using HogwartsPotions.Models.Dtos;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<List<Room>> GetAllRooms()
        {
            return await _roomService.GetAllRooms();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom([FromBody] RoomDto roomDto)
        {
            var room = new Room()
            {
                Capacity = roomDto.Capacity
            };

            await _roomService.AddRoom(room);

            return CreatedAtAction(
                nameof(GetRoomById),
                new { id = room.ID },
                room);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(long id)
        {
            var room = await _roomService.GetRoom(id);
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoomById(long id, [FromBody] RoomDto roomDto)
        {
            if (id != roomDto.ID)
            {
                return BadRequest();
            }

            var roomToUpdate = await _roomService.GetRoom(id);
            if (roomToUpdate == null)
            {
                return NotFound();
            }

            roomToUpdate.Capacity = roomDto.Capacity;

            try
            {
                await _roomService.UpdateRoom(roomToUpdate);
            }
            catch (DbUpdateConcurrencyException) when (_roomService.GetRoom(id) == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomById(long id)
        {
            var room = await _roomService.GetRoom(id);
            if (room == null)
            {
                return NotFound();
            }

            await _roomService.DeleteRoom(room);

            return NoContent();
        }

        [HttpGet("rat-owners")]
        public async Task<List<Room>> GetRoomsForRatOwners()
        {
            return await _roomService.GetRoomsForRatOwners();
        }
    }
}
