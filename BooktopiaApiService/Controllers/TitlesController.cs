using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooktopiaApiService.Models;

namespace BooktopiaApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitlesController : ControllerBase
    {
        private readonly BooktopiaContext _context;

        public TitlesController(BooktopiaContext context)
        {
            _context = context;
        }

        // GET: api/Titles
        [HttpGet]
        public IEnumerable<Title> GetTitle()
        {
            return _context.Title;
        }

        // GET: api/Titles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTitle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var title = await _context.Title.FindAsync(id);

            if (title == null)
            {
                return NotFound();
            }

            return Ok(title);
        }

        // PUT: api/Titles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTitle([FromRoute] int id, [FromBody] Title title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != title.TitleId)
            {
                return BadRequest();
            }

            _context.Entry(title).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TitleExists(id))
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

        // POST: api/Titles
        [HttpPost]
        public async Task<IActionResult> PostTitle([FromBody] Title title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Title.Add(title);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTitle", new { id = title.TitleId }, title);
        }

        // DELETE: api/Titles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTitle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var title = await _context.Title.FindAsync(id);
            if (title == null)
            {
                return NotFound();
            }

            _context.Title.Remove(title);
            await _context.SaveChangesAsync();

            return Ok(title);
        }

        private bool TitleExists(int id)
        {
            return _context.Title.Any(e => e.TitleId == id);
        }
    }
}