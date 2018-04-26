using System;
using System.Collections.Generic;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace NiceNuget.Api {
    public static class AmazonParameters {
        public static IWebHostBuilder AddAmazonParameters(this IWebHostBuilder builder) {
            // Lol
            var assmc = new AmazonSimpleSystemsManagementClient(RegionEndpoint.USEast2);

            // Grab params from the server
            var parameters = assmc.GetParametersByPathAsync(new GetParametersByPathRequest {
                Path = "/NiceNuget",
                Recursive = true,
                WithDecryption = true
            }).Result;

            // Transform them as needed
            var configList = new List<KeyValuePair<string, string>>();
            foreach (var param in parameters.Parameters) {
                var name = param.Name.Replace("/NiceNuget/", "").Replace('/', ':');
                configList.Add(KeyValuePair.Create(name, param.Value));
            }

            // Inject into config
            builder.ConfigureAppConfiguration((ContextBoundObject, config) => {
                config.AddInMemoryCollection(configList);
            });

            return builder;
        }
    }
}