using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231_API.Dtos;
using PRN231_API.Models;

namespace PRN231_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private PRN231_DBContext context;
        private IMapper mapper;

        public PagesController(PRN231_DBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{chapterId}")]
        public async Task<ActionResult<IEnumerable<Page>>> Get(int chapterId)
        {
            var pages = await context.Pages.Where(p => p.ChapterId == chapterId)
                .OrderBy(p => p.PageNumber).ToListAsync();
            return Ok(pages);
        }

        [HttpPost("{chapterId}")]
        public async Task<ActionResult> Post(int chapterId, IFormFile[] imgPage)
        {
            var chapter = await context.Chapters.FirstOrDefaultAsync(c => c.ChapterId == chapterId);
            if (chapter == null) return NotFound("This chapter doesn't exist!");
         
                for(int i = 0; i < imgPage.Length; i++)
                {
                using (var memoryStream = new MemoryStream())
                {
                    await imgPage[i].CopyToAsync(memoryStream);
                    byte[] imgByte = memoryStream.ToArray();
                    var page = new Page()
                    {
                        ChapterId = chapterId,
                        PageNumber = i,
                        Image = imgByte
                    };
                    await context.Pages.AddAsync(page);
                    await context.SaveChangesAsync();
                }
            }
            var pages = context.Pages.Where(p => p.ChapterId == chapterId).ToList();
            return Created("Page", pages);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, IFormFile file)
        {
            var page = await context.Pages.FirstOrDefaultAsync(p => p.PageId == id);
            if (page == null) return NotFound();
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] imgByte = memoryStream.ToArray();
                page.Image = imgByte;
            }
            context.Pages.Update(page);
            await context.SaveChangesAsync();
            return Ok(page);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var page = context.Pages.Where(p => p.ChapterId == id).ToList();
            if (page == null) return NotFound();
            context.Pages.RemoveRange(page);
            await context.SaveChangesAsync();
            return Ok("Delete successful!");
        }
        
    }
}
