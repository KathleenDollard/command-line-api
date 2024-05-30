// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;

namespace System.CommandLine.Suggest
{
    public class Program
    {
        internal static string DOTNET_SUGGEST_LOGGING = nameof(DOTNET_SUGGEST_LOGGING);

        public static int Main(string[] args)
        {
#if DEBUG
            LogDebug(new[] { "dotnet-suggest received: " }.Concat(args).ToArray());
#endif
            var provider = new CombineSuggestionRegistration(
                new GlobalToolsSuggestionRegistration(),
                new FileSuggestionRegistration());
            var dispatcher = new SuggestionDispatcher(provider);
            return dispatcher.Invoke(args);
        }

#if DEBUG
        internal static void LogDebug(params string[] args)
        {
            if (Environment.GetEnvironmentVariable(DOTNET_SUGGEST_LOGGING) == "1")
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dotnetSuggestFolder = Path.Combine(appData, "dotnet-suggest");

                Directory.CreateDirectory(dotnetSuggestFolder);

                var logFile = Path.Combine(dotnetSuggestFolder, "debug.log");
                File.AppendAllText(logFile, string.Join("|", args) + Environment.NewLine);
            }
        }
#endif
    }
}
