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
    public class UsersController : ControllerBase
    {
        private PRN231_DBContext context;
        private IMapper mapper;

        public UsersController(PRN231_DBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
            var users = await context.Users.ToListAsync();
            var usersDTO = mapper.Map<List<UserDTO>>(users);
            return Ok(usersDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await context.Users
                .Include(u => u.Comics)
                .ThenInclude(c => c.Chapters)
                .Include(u => u.ComicsNavigation)
                .FirstOrDefaultAsync(u => u.UserId == id);
            if(user.ComicsNavigation.FirstOrDefault(c => c.ComicId == 1) == null)
            {

            }
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(userDTO.Email.ToLower()));
                if(user != null) return BadRequest("This email was already used!");
                var createUser = mapper.Map<User>(userDTO);
                context.Users.Add(createUser);
                context.SaveChanges();
                return Created("Create",createUser);
            }
            return BadRequest("Input All field");
        }
        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            if (!string.IsNullOrEmpty(changePassword.newpassword) && !string.IsNullOrEmpty(changePassword.oldpassword)
                && !string.IsNullOrEmpty(changePassword.renewpassword))
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == 1);
                if (!user.Password.Equals(changePassword.oldpassword)) return Conflict("Nhập lại mật khẩu cũ");
                if (!changePassword.newpassword.Equals(changePassword.renewpassword)) return Conflict("Hai mật khẩu mới không trùng khớp");
                user.Password = changePassword.newpassword;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            return Ok(changePassword);
            }else return Conflict("Nhập hết các trường yêu cầu");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginForm login) {
            if (!ModelState.IsValid) return BadRequest("Input All field");
            var user = await context.Users.Include(u => u.Comics)
                .ThenInclude(c => c.Chapters)
                .Include(u => u.ComicsNavigation).FirstOrDefaultAsync(u => u.Email.Equals(login.Email) && u.Password.Equals(login.Password));
            if(user == null) return NotFound("Wrong email or password");
            return Ok(user);
        }
        [HttpPost("follow/{UserId}/{ComicId}")]
        public async Task<ActionResult> Follow(int UserId, int ComicId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == UserId);
            var comic = await context.Comics.FirstOrDefaultAsync(c => c.ComicId == ComicId);
            if (user == null || comic == null) return NotFound("Can't do this");
            user.ComicsNavigation?.Add(comic);
            context.SaveChanges();
            return Ok("Follow successful!");
        }
        [HttpPost("unfollow/{UserId}/{ComicId}")]
        public async Task<ActionResult> UnFollow(int UserId, int ComicId)
        {
            var user = await context.Users
                .Include(u => u.ComicsNavigation)
                .FirstOrDefaultAsync(u => u.UserId == UserId);
            if (user == null) return NotFound("Can't do this");
            var comic = user.ComicsNavigation?.FirstOrDefault(c => c.ComicId == ComicId);
            if (comic == null) return NotFound("you have not followed this comic yet!");
            user.ComicsNavigation?.Remove(comic);
            context.SaveChanges();
            return Ok("Unfollow successful!");
        }
    }
}
