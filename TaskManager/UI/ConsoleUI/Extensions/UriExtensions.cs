using System;

namespace Tasker.Extensions
{
    public static class UriExtensions
    {
        public static Uri CombineRelative(this Uri uri, string part)
        {
            if (!Uri.TryCreate($"{uri.OriginalString}/{part}", UriKind.Relative, out Uri combinedUri))
                throw new ArgumentException($"Could not create uri from {uri} and {part}");

            return combinedUri;
        }
    }
}