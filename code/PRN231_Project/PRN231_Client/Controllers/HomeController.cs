using Microsoft.AspNetCore.Mvc;
using PRN231_API.Dtos;
using PRN231_API.Helpers;
using PRN231_API.Models;
using PRN231_Client.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_Client.Controllers
{
    public class HomeController : Controller
    {
        private HttpClient client;
        private string comicApiUrl = "";
        private string userApiUrl = "";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            comicApiUrl = "http://localhost:5278/api/Comics";
            userApiUrl = "http://localhost:5278/api/Users";
        }
        private async Task<List<T>> GetDataObject<T>(string Apiurl)
        {
            using (var respone = await client.GetAsync(Apiurl))
            {
                if (respone.IsSuccessStatusCode)
                {
                    var strData = await respone.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<T>>(strData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                return new List<T>();
            }
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            List<Comic> comics = await GetDataObject<Comic>($"{comicApiUrl}/page/{page}");
            return View(comics);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginForm login)
        {
            if (ModelState.IsValid)
            {
                using(var respone = await client.PostAsJsonAsync($"{userApiUrl}/Login", login))
                {
                    if (respone.IsSuccessStatusCode)
                    {
                        var strData = await respone.Content.ReadAsStringAsync();
                        var user = JsonSerializer.Deserialize<User>(strData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        HttpContext.Session.SetObjectAsJson("user", user);
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                using (var respone = await client.PostAsJsonAsync($"{userApiUrl}", userDTO))
                {
                    if (respone.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Login));
                    }
                }
            }
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}