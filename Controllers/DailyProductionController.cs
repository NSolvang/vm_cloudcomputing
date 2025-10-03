using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using DailyProduction.Models;

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
            IWebHostEnvironment env)  // til at tjekke hvor vi kører
        {
            _logger = logger;

            // Hent stier fra konfiguration
            var localPath = configuration["CsvSettings:LocalPath"];
            var azurePath = configuration["CsvSettings:AzurePath"];

            // Bestem automatisk hvilken sti der skal bruges
            string filePath;
            if (env.IsDevelopment() || System.IO.File.Exists(Path.Combine(env.ContentRootPath, localPath)))
            {
                // Brug lokal fil
                filePath = Path.Combine(env.ContentRootPath, localPath);
            }
            else
            {
                // Brug Azure File Share sti
                filePath = azurePath;
            }

            _logger.LogInformation("Bruger CSV fil: {FilePath}", filePath);

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