using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NiceNuget.Api.Interfaces;
using NiceNuget.Api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace NiceNuget.Api.Infrastructure {
    public class NaivePackageCache : IPackageCache
    {
        // Maps contentIds (what will be requested) to keys (where package is stored)
        private readonly IDictionary<string, string> _contentPackageIds;
        private readonly List<PackageInfo> _packageInfo;
        private readonly IPackageProvider _provider;
        private DateTime _lastRefresh;

        public NaivePackageCache(IPackageProvider provider) {
            _provider = provider;
            _contentPackageIds = new Dictionary<string, string>();
            _packageInfo = new List<PackageInfo>();

            _lastRefresh = default(DateTime);
        }

        public async Task RefreshCache() {
            
            // Refresh once an hour max
            // Again we can make this better, but ok for now?
            if (DateTime.UtcNow - _lastRefresh < TimeSpan.FromHours(1)) {
                return;
            }

            var packageNames = await _provider.ListPackagesAsync("*.nupkg");
            var groups = from packageName in packageNames
                            where !packageName.EndsWith(".symbols.nupkg")
                            let idVersion = Utils.SplitIdAndVersion(Path.GetFileNameWithoutExtension(packageName))
                            select new {
                                Id = idVersion.Id,
                                Version = idVersion.Version,
                                FullFilePath = packageName,
                                ContentId = ((uint)packageName.GetHashCode()).ToString(),
                            }
                            into x
                            group x by x.Id;
            
            lock (_contentPackageIds) {
                _contentPackageIds.Clear();
                foreach (var group in groups) {
                    foreach (var info in group) {
                        _contentPackageIds[info.ContentId] = info.FullFilePath;
                    }
                }
            }

            var packages = (from g in groups
                let versions = g.Select(x => new VersionInfo { Version = x.Version, ContentId = x.ContentId}).OrderBy(x => x.Version).ToArray()
                    select new PackageInfo {
                        Id = g.Key,
                        Versions = versions,
                        LatestVersion = versions[versions.Length - 1].Version,
                        LatestContentId = versions[versions.Length - 1].ContentId
                    })
                    .OrderBy(x => x.Id)
                    .ToList();

            // Mmm
            lock (_packageInfo) {
                _packageInfo.Clear();
                _packageInfo.AddRange(packages);
            }

            _lastRefresh = DateTime.UtcNow;
        }

        public Task<Result<PackageInfo>> GetPackageInfoAsync(string packageId) {
            return Task.FromResult(GetPackageInfo(packageId));
        }

        private Result<PackageInfo> GetPackageInfo(string packageId) {
            var info = _packageInfo.SingleOrDefault(p => p.Id == packageId);
            if (info == null) {
                return Result.Fail<PackageInfo>("Package does not exist");
            }
            return Result.Ok(info);
        }

        public Task<Result<string>> GetPackageContentIdAsync(string packageId, Version version) {
            return Task.FromResult(GetPackageContentId(packageId, version));
        }

        public Result<string> GetPackageContentId(string packageId, Version version) {
            var info = _packageInfo.SingleOrDefault(p => p.Id == packageId);
            if (info == null) {
                return Result.Fail<string>("Package does not exist");
            }
            var versionInfo = info.Versions.SingleOrDefault(v => v.Version == version);
            if (versionInfo == null) {
                return Result.Fail<string>("Version does not exist");
            }
            return Result.Ok(versionInfo.ContentId);
        }

        public Task<ICollection<PackageInfo>> FindPackagesAsync(string query) {
            return Task.FromResult(FindPackages(query));
        }

        public ICollection<PackageInfo> FindPackages(string query) {
            return _packageInfo.Where(p => p.Id.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1).ToList();
        }

        public Task<Result<byte[]>> GetPackageAsync(string contentId) {
            return _provider.GetPackageAsync(_contentPackageIds[contentId]);
        }


    }
}