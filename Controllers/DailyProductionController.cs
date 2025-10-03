using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using DailyProduction.Models;
using System.IO;

namespace IbasAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyProductionController : ControllerBase
    {
        private readonly List<DailyProductionDTO> _productionRepo;
        private readonly ILogger<DailyProductionController> _logger;

        public DailyProductionController(
            ILogger<DailyProductionController> logger,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _logger = logger;

            // Hent stier fra konfiguration
            var localPath = configuration["CsvSettings:LocalPath"];
            var azurePath = configuration["CsvSettings:AzurePath"];

            string filePath;

            // Prioriter AzurePath, hvis den findes
            if (File.Exists(azurePath))
            {
                filePath = azurePath;
                _logger.LogInformation("Bruger Azure CSV fil: {FilePath}", filePath);
            }
            // Ellers prøv LocalPath (til udvikling)
            else if (File.Exists(Path.Combine(env.ContentRootPath, localPath)))
            {
                filePath = Path.Combine(env.ContentRootPath, localPath);
                _logger.LogInformation("Bruger lokal CSV fil: {FilePath}", filePath);
            }
            else
            {
                // Hvis ingen af dem findes, giv en klar fejl
                var msg = $"Ingen CSV-fil fundet. Tjek LocalPath og AzurePath.";
                _logger.LogError(msg);
                throw new FileNotFoundException(msg);
            }

            // Indlæs CSV
            _productionRepo = CsvLoader.LoadDailyProduction(filePath);
        }

        [HttpGet]
        public IEnumerable<DailyProductionDTO> Get()
        {
            return _productionRepo;
        }
    }
}