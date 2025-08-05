using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Frontendd.Models;
using Microsoft.AspNetCore.Authorization;

namespace Frontendd.Controllers
{
    public class HomeController : Controller
    {
        private const string ApiBaseUrl = "http://localhost:5243";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TimeTableForTeacher()
        {
            return View();
        }
        public IActionResult TimeTableForStudent()
        {
            return View();
        }

        public IActionResult BetaAi()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult VideoStreaming()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> StartWebcam(TblCamera req)
        {
           
            var url = $"{ApiBaseUrl}/Stream/start-webcam";
            using var client = new HttpClient();

            var response = await client.PostAsJsonAsync(url, req);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Webcam started!";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"Failed to start webcam: {error}";
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> StopStream(TblCamera req)
        {
            var url = $"{ApiBaseUrl}/Stream/stop-stream";
            using var client = new HttpClient();

            var response = await client.PostAsJsonAsync(url, req);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Stream stopped!";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"Failed to stop stream: {error}";
            }
            return View("Index");
        }
    }
}
