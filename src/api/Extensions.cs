using System;

namespace NiceNuget.Api {
    public static class VersionExtensions {
        public static Version Normalize(this Version version)
        {
            return new Version(
                Math.Max(0, version.Major),
                Math.Max(0, version.Minor),
                Math.Max(0, version.Build),
                Math.Max(0, version.Revision)
            );
        }
    }
}