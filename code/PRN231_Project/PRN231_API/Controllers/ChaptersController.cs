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
    public class ChaptersController : ControllerBase
    {
        private PRN231_DBContext context;
        private IMapper mapper;

        public ChaptersController(PRN231_DBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{ComicId}")]
        public async Task<ActionResult<IEnumerable<ChapterDTO>>> Get(int ComicId)
        {
            var chapters = context.Chapters.Where(c => c.ComicId == ComicId)
                .OrderByDescending(c => c.ChapterNumber);
            return Ok(chapters);
        }

        [HttpPost("{comicId}")]
        public async Task<ActionResult> Post(int comicId, ChapterDTO chapterDTO)
        {
            var comic = await context.Comics.FirstOrDefaultAsync(c => c.ComicId == comicId);
            if (comic == null) return NotFound("This comic doesn't exist!");
            var chapter = await context.Chapters
                .FirstOrDefaultAsync(c => c.ComicId == comicId && c.ChapterNumber == chapterDTO.ChapterNumber);
            if (chapter != null) return Conflict("This chapter number was exist!");
            var createdChapter = mapper.Map<Chapter>(chapterDTO);
            createdChapter.ComicId = comicId;
            context.Chapters.Add(createdChapter);
            context.SaveChanges();
            return Created("Created",createdChapter);
        }
    }
}
