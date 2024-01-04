using AutoMapper;
using DataAccess.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using PRN231_API.Dtos;
using PRN231_API.Models;

namespace PRN231_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComicsController : ControllerBase
    {
        private PRN231_DBContext context;
        private IMapper mapper;

        public ComicsController(PRN231_DBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comic>>> Get()
        {
            var comics = await context.Comics.ToListAsync();
            return Ok(comics);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comic>> GetById(int id)
        {
            var comic = await context.Comics
                .Include(c => c.Chapters)
                .Include(c => c.Categories)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ComicId == id);
            if (comic == null) return NotFound();
            return Ok(comic);
        }
        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<Comic>>> GetByName(string name)
        {
            var comics = await context.Comics
                .Where(c => c.ComicName.ToLower().Contains(name.ToLower()))
                .ToListAsync();
            return Ok(comics);
        }
        [HttpGet("totalPage")]
        public async Task<ActionResult<int>> GetTotalPage()
        {
            var comicPage = await GetComicsByPage(1, 8);
            var totalPage = comicPage.TotalPages;
            if (totalPage == 0) totalPage = 1;
            return Ok(totalPage);
        }
        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<Comic>>> GetPage(int page)
        {
            var ComicPage = await GetComicsByPage(page, 8);
            if (ComicPage == null) return NotFound();
            return Ok(ComicPage);
        }
        private async Task<PagedList<Comic>> GetComicsByPage(int page, int count)
        {
            var comics = context.Comics
                .Include(c => c.Chapters).AsQueryable<Comic>();
            var comicsPage = await PagedList<Comic>
                .Create(comics, page, count);
            return comicsPage;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ComicDTO comicDTO)
        {
            if(!ModelState.IsValid) return BadRequest("Please input all field");
            var comic = mapper.Map<Comic>(comicDTO);
            comic.Image = await ConvertImage(comicDTO.FileImage);
            comic.User = await context.Users.FirstOrDefaultAsync(u => u.UserId == comicDTO.UserId);
            context.Comics.Add(comic);
            context.SaveChanges();
            return Created("Create", comic);
        }

        [HttpPost("addCategory/{comicId}/{categoryId}")]
        public async Task<ActionResult> AddCategory(int comicId, int categoryId)
        {
            var comic = await context.Comics.FirstOrDefaultAsync(c => c.ComicId == comicId);
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (comic == null || category == null) return NotFound();
            comic.Categories?.Add(category);
            await context.SaveChangesAsync();
            return Ok();
        }
        private async Task<byte[]> ConvertImage(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] imgByte = memoryStream.ToArray();
                return imgByte;
            }
        }
    }
}
