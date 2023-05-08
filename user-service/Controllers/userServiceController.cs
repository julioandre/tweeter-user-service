using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_service.Data;
using user_service.Models;
using user_service.SyncDataServices.Http;

namespace user_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommandDataClient _commandDataClient;

        public userServiceController(ApplicationDbContext context, ICommandDataClient commandDataClient)
        {
            _context = context;
            _commandDataClient = commandDataClient;
        }

        // GET: api/userService
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
            try
            {
                await _commandDataClient.SendUserToFollow("Juser");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--- Error: {ex.Message} ---");
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/userService/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserEntity>> GetUserEntity(int id)
        {
            var userEntity = await _context.Users.FindAsync(id);

            if (userEntity == null)
            {
                return NotFound();
            }

            return userEntity;
        }

        // PUT: api/userService/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserEntity(string id, UserEntity userEntity)
        {
            if (id != userEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(userEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/userService
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserEntity>> PostUserEntity(UserEntity userEntity)
        {
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserEntity", new { id = userEntity.Id }, userEntity);
        }

        // DELETE: api/userService/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserEntity(int id)
        {
            var userEntity = await _context.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserEntityExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
