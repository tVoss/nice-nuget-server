using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NiceNuget.Api.Infrastructure;
using NiceNuget.Api.Interfaces;
using NiceNuget.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NiceNuget.Api.Controllers {
    
    [Route("v2")]
    public class Version2Controller : ControllerBase {
        private readonly IPackageCache _cache;
        private readonly XmlFormatter _formatter;
        private readonly ILogger<Version2Controller> _logger;

        public Version2Controller(IPackageCache cache, XmlFormatter formatter, ILogger<Version2Controller> logger) {
            _cache = cache;
            _formatter = formatter;
            _logger = logger;           
        }

        [HttpGet("download/{contentId}")]
        public async Task<IActionResult> DownloadPackage(string contentId) {
            var package = await _cache.GetPackageAsync(contentId);
            if (!package.Success) {
                _logger.LogWarning($"Could not get package {contentId}");
                _logger.LogWarning(package.Error);
                return NotFound();
            }
            return File(package.Value, "application/octet-stream");
        }

        [HttpGet("search()")]
        public async Task<ActionResult> SearchPackages([FromQuery] int skip, [FromQuery()] int top, [FromQuery] string searchTerm) {
            // Get em
            var packages = await _cache.FindPackagesAsync(searchTerm);
            
            // Clean em
            packages = packages.Skip(skip).ToList();
            packages = packages.Take(Math.Min(top, packages.Count())).ToList();
            
            // Send em
            var xml = _formatter.CreatePackageFeed(packages, "/v2");
            return Ok(xml.ToString());
        }

        [HttpGet("Packages({query})")]
        public async Task<ActionResult> GetPackages(string query) {
            var kvs = query.Split(Characters.Comma).Select(p => p.Split('=', 2));
            var idKv = kvs.SingleOrDefault(kv => string.Equals(kv[0], "id", StringComparison.OrdinalIgnoreCase));
            var versionKv = kvs.SingleOrDefault(kv => string.Equals(kv[0], "version", StringComparison.OrdinalIgnoreCase));

            if (idKv == null || versionKv == null) {
                return BadRequest();
            }

            var id = idKv[1].Trim(Characters.SingleQuote);
            Version.TryParse(versionKv[1], out var version);

            if (string.IsNullOrWhiteSpace(id) || version == null) {
                return BadRequest();
            }

            version = version.Normalize();
            var contentId = await _cache.GetPackageContentIdAsync(id, version);

            if (!contentId.Success) {
                return NotFound();
            }

            var xml = _formatter.CreatePackageEntry(id, contentId.Value, version, "/v2");

            return Ok(xml.ToString());
        }

        [HttpGet("FindPackagesById()")]
        public async Task<ActionResult> FindPackageById([FromQuery] string id) {
            _logger.LogInformation($"Finding package with id {id}");

            var packageInfo = await _cache.GetPackageInfoAsync(id.Trim(Characters.SingleQuote));
            var versions = packageInfo.Success ? packageInfo.Value.Versions : Globals.EmptyVersionInfoArray;

            _logger.LogInformation($"{versions.Length} versions were returned for {id}");

            var xml = _formatter.CreateVersionFeed(id, versions, "/v2");
            return Ok(xml.ToString());
        }

    }
}