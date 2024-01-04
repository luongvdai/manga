using Microsoft.AspNetCore.Mvc;
using PRN231_API.Dtos;
using PRN231_API.Helpers;
using PRN231_API.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_Client.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient client;
        private string userApiUrl = "";
        private string comicApiUrl = "";

        public UserController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            userApiUrl = "http://localhost:5278/api/Users";
            comicApiUrl = "http://localhost:5278/api/Comics";
        }

        public async Task<ActionResult> Profile()
        {
            var user = HttpContext.Session.GetObjectFromJson<User>("user");
            if (user == null) return RedirectToAction("Login", "Home");
            using (var respone = await client.GetAsync($"{userApiUrl}/{user.UserId}"))
            {
                if (respone.IsSuccessStatusCode)
                {
                    var strData = await respone.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<User>(strData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    HttpContext.Session.SetObjectAsJson("user", user);
                    return View(user);
                }
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadComic(ComicDTO comicDTO)
        {
            using(var respone = await client.PostAsJsonAsync($"{comicApiUrl}", comicDTO))
            {
                    return RedirectToAction(nameof(Profile));
            }
        }
    }
}
