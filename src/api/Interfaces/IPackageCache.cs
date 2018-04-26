using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NiceNuget.Api.Infrastructure;
using NiceNuget.Api.Models;

namespace NiceNuget.Api.Interfaces {
    public interface IPackageCache {
        Task RefreshCache();
        Task<Result<PackageInfo>> GetPackageInfoAsync(string packageId);
        Task<Result<string>> GetPackageContentIdAsync(string packageId, Version version);
        Task<ICollection<PackageInfo>> FindPackagesAsync(string query);
        Task<Result<byte[]>> GetPackageAsync(string contentId);
    }
}