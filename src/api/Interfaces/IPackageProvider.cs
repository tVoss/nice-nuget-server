using System.Collections.Generic;
using System.Threading.Tasks;
using NiceNuget.Api.Infrastructure;
using NiceNuget.Api.Models;

namespace NiceNuget.Api.Interfaces {
    public interface IPackageProvider {
        
        // Get package from remote source
        Task<Result<byte[]>> GetPackageAsync(string path);

        // If a package exists or not
        Task<bool> PackageExistsAsync(string path);

        // Get list of all package names
        Task<ICollection<string>> ListPackagesAsync(string search = null);
    }
}