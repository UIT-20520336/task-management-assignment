using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Models;
using Microsoft.AspNetCore.Authorization;
using webapi.Services;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskContext _context;
        public TasksController(TaskContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetTasks()
        {
          if (_context.Tasks == null)
          {
              return NotFound();
          }
            var tasks = await _context.Tasks
        .Select(t => new Models.Task
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            UserId = t.UserId
        })
        .ToListAsync();

            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> GetTask(int id)
        {
          if (_context.Tasks == null)
          {
              return NotFound();
          }
            var task = await _context.Tasks
        .Where(t => t.Id == id)
        .Select(t => new Models.Task
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            UserId = t.UserId
        })
        .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }



        // PUT: api/Tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var updatedTask = new Models.Task
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                UserId = task.UserId
            };

            return Ok(await _context.Tasks.Include(t => t.User).ToListAsync());
        }

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Models.Task>>> PostTask(Models.Task task)
        {
         
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'TaskContext.Tasks' is null.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            task.UserId = int.Parse(userId);
            var newTask = new Models.Task
            {
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                UserId = task.UserId
            };

            _context.Tasks.Add(newTask);

            var isUserTracked = _context.ChangeTracker.Entries<User>()
        .Any(e => e.State == EntityState.Added);

            await _context.SaveChangesAsync();

            var userTasks = await _context.Tasks
                .Where(t => t.UserId == newTask.UserId)
                .Select(t => new Models.Task
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    UserId = t.UserId
                })
                .ToListAsync();

            
            return Ok(userTasks);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (_context.Tasks == null)
            {
                return NotFound();
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(await _context.Tasks.Include(t => t.User).ToListAsync());
        }

        // PATCH: api/Tasks/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTaskStatusAsync(int id, [FromBody] Models.Task taskUpdate)
        {
            
            if (!ModelState.IsValid || taskUpdate.IsCompleted == null)
            {
                return BadRequest(new
                {
                    Title = "One or more validation errors occurred.",
                    Status = 400,
                    Errors = ModelState
                });
            }

            
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound(new { Message = "Task not found" });
            }

            
            if (task.Title != null)
            {
                task.Title = taskUpdate.Title;
            }

            if (task.Description != null)
            {
                task.Description = taskUpdate.Description;
            }

            if (taskUpdate.IsCompleted.HasValue)
            {
                task.IsCompleted = taskUpdate.IsCompleted.Value;
            }
            _context.SaveChanges();

            return Ok(await _context.Tasks.Include(t => t.User).ToListAsync());
        }

        private bool TaskExists(int id)
        {
            return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
