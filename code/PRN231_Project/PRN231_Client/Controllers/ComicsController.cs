using Microsoft.AspNetCore.Mvc;
using PRN231_API.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PRN231_Client.Controllers
{
    public class ComicsController : Controller
    {
        private HttpClient client;
        private string comicApiUrl = "";
        private string pageApiUrl = "";
        private string chapterApiUrl = "";
        public ComicsController() {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            comicApiUrl = "http://localhost:5278/api/Comics";
            pageApiUrl = "http://localhost:5278/api/Pages";
            chapterApiUrl = "http://localhost:5278/api/Chapters";
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
        public async Task<IActionResult> Index(int ComicId)
        {
            using(var respone = await client.GetAsync($"{comicApiUrl}/{ComicId}"))
            {
                if (respone.IsSuccessStatusCode)
                {
                    var strData = await respone.Content.ReadAsStringAsync();
                    var comic = JsonSerializer.Deserialize<Comic>(strData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
                    return View(comic);
                }
                return NotFound("Can't found your request!");
            }
        }
        //[Route("Comics/{ComicId}/{ChapterId}")]
        public async Task<IActionResult> Chapter(int ComicId, int ChapterId)
        {
            var page = await GetDataObject<Page>($"{pageApiUrl}/{ChapterId}");
            ViewData["Chapters"] = await GetDataObject<Chapter>($"{chapterApiUrl}/{ComicId}");
            return View(page);
        }
    }
}
