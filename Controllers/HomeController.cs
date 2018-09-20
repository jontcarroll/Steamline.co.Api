using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Steamline.co.Models;
using Steamline.co.Content.Helpers;
using Newtonsoft.Json;
using System.IO;

namespace Steamline.co.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
        private static InMemoryCache _inMemoryCache;
        private static InMemoryCache Cache
        {
            get
            {
                if (_inMemoryCache == null)
                    _inMemoryCache = new InMemoryCache();
                return _inMemoryCache;
            }
        }


        public ActionResult GetGamesFromProfileUrl(string url, string groupCode)
        {
            var steamId = SteamApiHelper.Get64BitSteamId(url);
            var games = SteamApiHelper.GetGamesFromProfile(steamId);

            var vm = new ProfileViewModel
            {
                Games = games,
                GroupCode = string.IsNullOrEmpty(groupCode) ? GetGroupCode() : groupCode
            };

            return View("_ProfileView", vm);
        }

        public JsonResult GetGameDetails(long appId)
        {
            // TODO: Client should make this directly to Steam
            try
            {
                var cached = Cache.GetOrSet(appId.ToString(), () => SteamApiHelper.GetGameDetails(appId));

                return new JsonResult(JsonConvert.SerializeObject(cached));
            }
            catch (Exception ex)
            {
                var ex2 = ex.Message;
                return null;
            }
        }

        private string GetGroupCode()
        {
            // string path = Server.MapPath("~/Content/Resources/words.txt");

            // List<string> lines = new List<string>();
            // Random rnd = new Random();

            // if (System.IO.File.Exists(path))
            // {
            //     StreamReader reader = new StreamReader(path);

            //     while (!(reader.Peek() == -1))
            //         lines.Add(reader.ReadLine());


            //     reader.Close();
            //     reader.Dispose();

            //     // Returns three random words with uppercase first letter
            //     string word1 = lines[rnd.Next(lines.Count)].Trim().FirstCharToUpper();
            //     string word2 = lines[rnd.Next(lines.Count)].Trim().FirstCharToUpper();
            //     string word3 = lines[rnd.Next(lines.Count)].Trim().FirstCharToUpper();

            //     // Strip all non-alpha characters from unique code
            //     return new string($"{word1}{word2}{word3}".Where(char.IsLetter).ToArray());
            // }
            // else
            // {
                return string.Empty;
            // }
        }
    }
}
