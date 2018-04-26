using System;

namespace NiceNuget.Api.Models {
    public class PackageInfo {
        public string Id;
        public Version LatestVersion;
        public string LatestContentId;
        public VersionInfo[] Versions;
    }
}