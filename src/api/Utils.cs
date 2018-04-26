using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using NiceNuget.Api.Models;

namespace NiceNuget.Api {
    public static class Utils
    {
        public static IdVersion SplitIdAndVersion(string filename)
        {
            var parts = filename.Split(Characters.Dot);

            var versionParts = new List<int>();
            int num;

            int i;
            for (i = parts.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(parts[i], out num) == false)
                    break;

                versionParts.Insert(0, num);
            }

            if (versionParts.Count < 4)
                versionParts.AddRange(Enumerable.Repeat(0, 4 - versionParts.Count));

            return new IdVersion
            {
                Id = string.Join(".", parts, 0, i + 1),
                Version = new Version(versionParts[0], versionParts[1], versionParts[2], versionParts[3])
            };
        }
    }
}

