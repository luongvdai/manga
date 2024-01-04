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
    public class CategoriesController : ControllerBase
    {
        private PRN231_DBContext context;
        private IMapper mapper;

        public CategoriesController(PRN231_DBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            var categories = await context.Categories.ToListAsync();
            var categoriesDTO = mapper.Map<List<CategoryDTO>>(categories);
            return Ok(categoriesDTO);
        }

        [HttpGet("id")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return NotFound();
            var categoryDTO = mapper.Map<CategoryDTO>(category);
            return Ok(categoryDTO);
        }

    }
}
