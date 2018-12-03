using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooktopiaApiService.Models;
using System.Globalization;

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

        // GET: api/Titles/title/{title}
        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetTitlesByTitle([FromRoute] string title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var titles = await _context.Title.Where(t => t.Title1 == title).OrderByDescending(t => t.PubDate).ToListAsync();

            if (titles == null)
            {
                return NotFound();
            }

            return Ok(titles);
        }

        // GET: api/Titles/isbn/{isbn}
        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> GetTitlesByIsbn([FromRoute] string isbn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var titles = await _context.Title.Where(t => t.Isbn == isbn).OrderBy(t => t.Title1).ToListAsync();

            if (titles == null)
            {
                return NotFound();
            }

            return Ok(titles);
        }

        // GET: api/Titles/author/{author}
        [HttpGet("author/{author}")]
        public async Task<IActionResult> GetTitlesByAuthor([FromRoute] string author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var titles = await _context.Title.Where(t => t.Author == author).OrderByDescending(t => t.PubDate).ToListAsync();

            if (titles == null)
            {
                return NotFound();
            }

            return Ok(titles);
        }

        // GET: api/Titles/publisher/{publisher}
        [HttpGet("publisher/{publisher}")]
        public async Task<IActionResult> GetTitlesByPublisher([FromRoute] string publisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var titles = await _context.Title.Where(t => t.Publisher == publisher).OrderBy(t => t.Title1).ToListAsync();

            if (titles == null)
            {
                return NotFound();
            }

            return Ok(titles);
        }

        // GET: api/Titles/genre/{genre}
        [HttpGet("genre/{genre}")]
        public async Task<IActionResult> GetTitlesByGenre([FromRoute] string genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var titles = await _context.Title.Where(t => t.Genre == genre).OrderBy(t => t.Title1).ToListAsync();

            if (titles == null)
            {
                return NotFound();
            }

            return Ok(titles);
        }

        // GET: api/Titles/pubdate/{startPubDate}/{endPubDate}
        [HttpGet("pubdate/{startPubDate}/{endPubDate}")]
        public async Task<IActionResult> GetTitlesByPubdate([FromRoute] string startPubDate, string endPubDate)
        {
            var sDate = DateTime.ParseExact(startPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            var eDate = DateTime.ParseExact(endPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);

            if (!ModelState.IsValid || sDate > eDate)
            {
                return BadRequest(ModelState);
            }

            var titles = await _context.Title.Where(t => t.PubDate > sDate && t.PubDate < eDate).OrderByDescending(t => t.PubDate).ToListAsync();

            if (titles == null)
            {
                return NotFound();
            }

            return Ok(titles);
        }


        // GET: api/Titles/price/{minPrice}/{maxPrice}
        [HttpGet("price/{minPrice}/{maxPrice}")]
        public async Task<IActionResult> GetTitlesByPrice([FromRoute] int minPrice, int maxPrice)
        {
            if (!ModelState.IsValid || minPrice > maxPrice)
            {
                return BadRequest(ModelState);
            }

            var titles = await _context.Title.Where(t => t.Price >= minPrice && t.Price <= maxPrice).OrderByDescending(t => t.Price).ToListAsync();

            if (titles == null)
            {
                return NotFound();
            }

            return Ok(titles);
        }


        // GET: api/Titles/top/{startPubDate}/{endPubDate}/{count}
        [HttpGet("top/{startPubDate}/{endPubDate}/{count}")]
        public async Task<IActionResult> GetTopTitlesBySeason([FromRoute] string startPubDate, string endPubDate, int count)
        {
            var sDate = DateTime.ParseExact(startPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            var eDate = DateTime.ParseExact(endPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);

            if (!ModelState.IsValid || sDate > eDate)
            {
                return BadRequest(ModelState);
            }

            // Select all titles and orders by orderdates
            var items = await _context.Title
                .Join(_context.OrderDetail, t => t.TitleId,
                od => od.TitleId, (t, od) => new { Title = t, OrderDetail = od })
                .Join(_context.Order, titleAndOrderDetail => titleAndOrderDetail.OrderDetail.OrderId,
                o => o.OrderId, (titleAndOrderDetail, o)
                => new { titleAndOrderDetail.Title, titleAndOrderDetail.OrderDetail, Order = o })
                .Where(item => item.Order.OrderDate >= sDate && item.Order.OrderDate <= eDate)
                .GroupBy(g => g.Title)
                .Select(i => new
                {
                    i.First().Title,
                    COUNT = i.Sum(c => c.OrderDetail.Count)
                }).Take(count).OrderByDescending(t => t.COUNT).ToListAsync();


            if (items == null)
            {
                return NotFound();
            }

            return Ok(items);
        }

        // GET: api/Titles/top/genres/{startPubDate}/{endPubDate}/{count}
        [HttpGet("top/genres/{startPubDate}/{endPubDate}/{count}")]
        public async Task<IActionResult> GetTopGenresBySeason([FromRoute] string startPubDate, string endPubDate, int count)
        {
            var sDate = DateTime.ParseExact(startPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            var eDate = DateTime.ParseExact(endPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);

            if (!ModelState.IsValid || sDate > eDate)
            {
                return BadRequest(ModelState);
            }

            // Select all titles and orders by orderdates
            var items = await _context.Title
                .Join(_context.OrderDetail, t => t.TitleId,
                od => od.TitleId, (t, od) => new { Title = t, OrderDetail = od })
                .Join(_context.Order, titleAndOrderDetail => titleAndOrderDetail.OrderDetail.OrderId,
                o => o.OrderId, (titleAndOrderDetail, o)
                => new { titleAndOrderDetail.Title, titleAndOrderDetail.OrderDetail, Order = o })
                .Where(item => item.Order.OrderDate >= sDate && item.Order.OrderDate <= eDate)
                .GroupBy(g => g.Title.Genre)
                .Select(i => new
                {
                    i.First().Title.Genre,  // group by genre and get the first item in the same genre, and return the first book's genre.
                                            // but all the genres of the books in the same genre are all same, so it gets only the first book's genre.
                    COUNT = i.Sum(c => c.OrderDetail.Count)
                }).Take(count).OrderByDescending(t => t.COUNT).ToListAsync();


            if (items == null)
            {
                return NotFound();
            }

            return Ok(items);
        }

        // GET: api/Titles/top/authors/{startPubDate}/{endPubDate}/{count}
        [HttpGet("top/authors/{startPubDate}/{endPubDate}/{count}")]
        public async Task<IActionResult> GetTopAuthorsBySeason([FromRoute] string startPubDate, string endPubDate, int count)
        {
            var sDate = DateTime.ParseExact(startPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            var eDate = DateTime.ParseExact(endPubDate, "yyyyMMdd", CultureInfo.InvariantCulture);

            if (!ModelState.IsValid || sDate > eDate)
            {
                return BadRequest(ModelState);
            }

            // Select all titles and orders by orderdates
            var items = await _context.Title
                .Join(_context.OrderDetail, t => t.TitleId,
                od => od.TitleId, (t, od) => new { Title = t, OrderDetail = od })
                .Join(_context.Order, titleAndOrderDetail => titleAndOrderDetail.OrderDetail.OrderId,
                o => o.OrderId, (titleAndOrderDetail, o)
                => new { titleAndOrderDetail.Title, titleAndOrderDetail.OrderDetail, Order = o })
                .Where(item => item.Order.OrderDate >= sDate && item.Order.OrderDate <= eDate)
                .GroupBy(g => g.Title.Author)
                .Select(i => new
                {
                    i.First().Title.Author,  // group by author and get the first item in the same author, and return the first book's author.
                                             // but all the authors of the books in the same author are all same, so it gets only the first book's author.
                    COUNT = i.Sum(c => c.OrderDetail.Count)
                }).Take(count).OrderByDescending(t => t.COUNT).ToListAsync();


            if (items == null)
            {
                return NotFound();
            }

            return Ok(items);
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