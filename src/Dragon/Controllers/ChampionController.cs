using System;
using System.Linq;
using System.Threading.Tasks;
using Dragon.Services;
using Microsoft.AspNetCore.Mvc;
using RiotPls.DataDragon;
using RiotPls.DataDragon.Entities;
using RiotPls.DataDragon.Enums;
using RiotPls.DataDragon.Extensions;

namespace Dragon.Controllers
{
    [Route("[controller]")]
    public class ChampionController : DragonController
    {
        private readonly DataDragonService _service;

        public ChampionController(DataDragonService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public IActionResult GetChampionsAsync()
        {
            ViewData["language_code"] = _service.Client.DefaultLanguage.GetCode();
            return Redirect($"/champion/{_service.Client.DefaultLanguage.GetCode()}");
        }

        [HttpGet("{lang}")]
        public async Task<IActionResult> GetChampionsLangAsync(string lang)
        {
            var language = lang.GetLanguage();
            var languages = await _service.Client.GetLanguagesAsync();
            
            var latestVersion = await _service.Client.GetVersionsAsync();
            var champions = await _service.Client.GetChampionsAsync(latestVersion.First(), language);

            ViewData["champions"] = champions;
            ViewData["language_code"] = lang;
            ViewData["languages"] = languages;
            
            return View("Champions");
        }
        
        [HttpGet("{lang}/{name}")]
        public async Task<IActionResult> GetChampionAsync(string lang = "", string name = "")
        {
            var language = lang.GetLanguage();

            var latestVersion = await _service.Client.GetVersionsAsync();
            var champions = await _service.Client.GetChampionsAsync(latestVersion.First(), language);

            Champion champion;
            if (!Enum.TryParse<ChampionId>(name, true, out var id))
            {
                champion = champions.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCulture));   
            }
            else
            {
                champion = champions.FirstOrDefault(x => x.Id == id);
            }

            if (champion is null)
            {
                return Redirect("/");
            }

            ViewData["champion"] = champion;
            return View("Champion");
        }
    }
}