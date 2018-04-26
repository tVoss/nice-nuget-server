using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using NiceNuget.Api.Interfaces;
using NiceNuget.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NiceNuget.Api.Infrastructure {
    public class S3PackageProvider : IPackageProvider {

        private readonly AmazonS3Client _s3Client;
        private readonly string _bucket;
        private readonly ILogger<S3PackageProvider> _logger;

        public S3PackageProvider(AmazonS3Client s3Client, IConfiguration config, ILogger<S3PackageProvider> logger) {
            _s3Client = s3Client;
            _bucket = config["Aws:Bucket"];
            _logger = logger;

            _logger.LogDebug($"Created S3PackageProvider with bucket {_bucket}");
        }

        public async Task<Result<byte[]>> GetPackageAsync(string id) {
            _logger.LogInformation($"Getting package with id {id}");

            var req = new GetObjectRequest {
                BucketName = _bucket,
                Key = id
            };

            try {
                using (var res = await _s3Client.GetObjectAsync(req))
                using (var ms = new MemoryStream()) {
                    res.ResponseStream.CopyTo(ms);

                    _logger.LogInformation($"Successfully got package with id {id}");

                    return Result.Ok(ms.ToArray());
                }
            } catch (Exception e) {
                return Result.Fail<byte[]>(e.Message);
            }
        }

        public async Task<ICollection<string>> ListPackagesAsync(string search = null) {
            _logger.LogInformation("Listing all packages" + search == null ? "" : $" with search string {search}");

            var req = new ListObjectsV2Request {
                BucketName = _bucket
            };

            var res = await _s3Client.ListObjectsV2Async(req);
            var packages = res.S3Objects.Select(o => o.Key);
            if (search != null) {
                var regex = new Regex(search.Replace(".", "\\.").Replace("*", ".*"));
                packages = packages.Where(k => regex.IsMatch(k));
            }
            var names = packages.ToList();

            _logger.LogInformation($"Successfully listed {names.Count} packages");
            return names;
        }

        public async Task<bool> PackageExistsAsync(string id) {
            _logger.LogInformation($"Checking if package id {id} exists");
            var req = new ListObjectsV2Request {
                BucketName = _bucket
            };

            var res = await _s3Client.ListObjectsV2Async(req);
            var exists = res.S3Objects.Any(o => o.Key == id);
        
            _logger.LogInformation($"Package id {id} does{(exists ? "" : " not")} exist");

            return exists;
        }

        public void ProcessPackages()
        {
            throw new System.NotImplementedException();
        }
        public void FindPackageContentId(string id)
        {
            throw new System.NotImplementedException();
        }

        public void FindPackageIndex(string id)
        {
            throw new System.NotImplementedException();
        }

    }
}