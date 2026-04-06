using System;
using System.Collections.Generic;
using System.Text;

namespace BuenosAiresExp.Services
{
    public static class PathHelper
    {
        private readonly static string _appRoot = GetAppRoot();

        private static string GetAppRoot()
        {
            var processPath = Environment.ProcessPath;
            if(!string.IsNullOrEmpty(processPath))
                return Path.GetDirectoryName(processPath)!;

            return AppContext.BaseDirectory;
        }

        public static string Assets(params string[] segments)
            => Path.Combine(new[] { _appRoot }.Concat(segments).ToArray());

        public static string Icon(string fileName)
            => Assets("Assets", "baexp-icons", "png", fileName);

        public static string Logo(string fileName)
            => Assets("Assets", "baexp-logo-system", "logos", "png", fileName);

        public static string WindowIcon(string fileName)
            => Assets("Assets", "baexp-logo-system", "icons", "ico", fileName);
    }
}
