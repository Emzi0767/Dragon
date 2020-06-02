using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dragon.Services;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _environment;
        private readonly HttpClient _httpClient;

        public ChampionController(
            DataDragonService service, 
            IHttpClientFactory httpClientFactory, 
            IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
            _httpClient = httpClientFactory.CreateClient("Dragon");
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

            var championTiles = champions.Select(GetTilePathAsync);
            
            ViewData["champions"] = champions;
            ViewData["language_code"] = lang;
            ViewData["languages"] = languages;
            ViewData["Title"] = "Champion List";
            
            var tiles = await Task.WhenAll(championTiles);
            ViewData["tiles"] = tiles.ToDictionary(
                x => x.Item1, 
                y => y.Item2);
            
            return View("Champions");
        }
        
        [HttpGet("{lang}/{name}")]
        public async Task<IActionResult> GetChampionAsync(string lang = "", string name = "")
        {
            var language = lang.GetLanguage();

            var latestVersion = await _service.Client.GetVersionsAsync();
            var champions = await _service.Client.GetChampionsAsync(latestVersion.First(), language);

            var champion = !Enum.TryParse<ChampionId>(name, true, out var id) 
                ? champions.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCulture)) 
                : champions.FirstOrDefault(x => x.Id == id);

            if (champion is null)
            {
                return Redirect("/");
            }
            
            var skinTask = champion.Skins.Select(GetSkinPathAsync);
            var spellsTask = Enum.GetValues(typeof(ChampionAbility))
                .Cast<ChampionAbility>()
                .Skip(0)
                .Select(x => GetSpellPathAsync(champion, x));

            var skins = await Task.WhenAll(skinTask);
            ViewData["skins"] = skins.ToDictionary(
                x => x.Item1, 
                y => y.Item2);
            
            ViewData["spells"] = await Task.WhenAll(spellsTask);
            ViewData["champion"] = champion;
            ViewData["Title"] = champion.Name;
            
            return View("Champion");
        }

        public async Task<string> GetSpellPathAsync(Champion champion, ChampionAbility spell)
        {
            var path = Path.Combine(_environment.WebRootPath, "spells");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            if (!System.IO.File.Exists($"{path}/{champion.Key}_{spell}.png"))
            {
                var content = await _httpClient.GetByteArrayAsync(champion.GetAbilityIconUrl(spell));
                await System.IO.File.WriteAllBytesAsync($"{path}/{champion.Key}_{spell}.png", content);
            }
            
            return $"/spells/{champion.Key}_{spell}.png"; 
        }
        
        public async Task<(int, string)> GetSkinPathAsync(ChampionSkin skin)
        {
            var path = Path.Combine(_environment.WebRootPath, "skins");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            if (!System.IO.File.Exists($"{path}/{skin.Champion.Key}_{skin.SkinIndex}.jpg"))
            {
                var content = await _httpClient.GetByteArrayAsync(skin.GetSplashArtUrl());
                await System.IO.File.WriteAllBytesAsync($"{path}/{skin.Champion.Key}_{skin.SkinIndex}.jpg", content);
            }
            
            return (skin.SkinIndex, $"/skins/{skin.Champion.Key}_{skin.SkinIndex}.jpg"); 
        }

        public async Task<(int, string)> GetTilePathAsync(Champion champion)
        {
            var path = Path.Combine(_environment.WebRootPath, "tiles");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            if (!System.IO.File.Exists($"{path}/{champion.Key}.jpg"))
            {
                var content = await _httpClient.GetByteArrayAsync(champion.GetTileUrl());
                await System.IO.File.WriteAllBytesAsync($"{path}/{champion.Key}.jpg", content);
            }

            return (champion.Key, $"/tiles/{champion.Key}.jpg");
        }
    }
}