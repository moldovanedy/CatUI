using System;
using System.IO;

namespace CatUI.Data
{
    /// <summary>
    /// Provides a cross-platform way of representing absolute paths for files (note that some Windows-specific or
    /// MS-DOS paths might not be representable using this object).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Despite its name, this object can represent directories as well by just having a "/" at the end of the
    /// <see cref="NormalizedPath"/>.
    /// </para>
    /// <para>
    /// Note that this object does very few checks to see if the path is valid, so it might still work even if the
    /// paths are invalid on the runtime platform.
    /// </para>
    /// </remarks>
    public class FilePath
    {
        /// <summary>
        /// This is the path represented in the platform-specific way. Generally, this is a string (on all desktop
        /// platforms), but it can also be another type of object. See remarks for more info. If you interact with
        /// platform-specific APIs, this is the property you'll generally want to use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// On Unix-like systems (Linux, macOS, etc.), this is the same as <see cref="NormalizedPath"/>, and they can be
        /// used interchangeably.
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item>
        /// On Windows: a string starting with the drive letter (e.g. C, D), followed by ":\", then by the actual
        /// absolute file path. It always uses backslashes ("\").
        /// </item>
        /// <item>On Linux: a string containing the absolute file path, starting with "/".</item>
        /// </list>
        /// </para>
        /// </remarks>
        public object NativePath { get; private set; }

        /// <summary>
        /// This is the cross-platform representation of the path. It always starts with a "/", even on Windows (e.g.
        /// "/C:/Windows/System32"), and always contains forward slashes only ("/").
        /// </summary>
        /// <remarks>
        /// On Unix-like systems (Linux, macOS, etc.), this is the same as <see cref="NativePath"/>, and they can be
        /// used interchangeably.
        /// </remarks>
        public string NormalizedPath { get; private set; }

        /// <summary>
        /// Returns only the file name, without the entire path.
        /// </summary>
        public string FileName => NormalizedPath.Substring(NormalizedPath.LastIndexOf('/') + 1);

        /// <summary>
        /// Constructs a file path from a normalized, cross-platform path (see <see cref="NormalizedPath"/>).
        /// </summary>
        /// <param name="normalizedPath"></param>
        public FilePath(string normalizedPath)
        {
            NormalizedPath = normalizedPath;
            if (string.IsNullOrEmpty(normalizedPath))
            {
                NativePath = "";
                return;
            }

            if (OperatingSystem.IsWindows())
            {
                NativePath = normalizedPath.Remove(0, 1).Replace('/', '\\');
            }
            else
            {
                NativePath = normalizedPath;
            }
        }

        /// <summary>
        /// Constructs a file path from a given native path. This throws an <see cref="ArgumentException"/> if the
        /// normalized path could not be constructed.
        /// </summary>
        /// <param name="nativePath"></param>
        /// <param name="isDirectory"></param>
        public FilePath(object nativePath, bool isDirectory)
        {
            NativePath = nativePath;

            if (OperatingSystem.IsWindows())
            {
                if (nativePath is not string pathAsString)
                {
                    throw new ArgumentException(
                        $"{nameof(nativePath)} is not a string. On Windows, the path must be a string.");
                }

                pathAsString = pathAsString.Replace('\\', '/');
                if (isDirectory && !pathAsString.EndsWith('/'))
                {
                    pathAsString += '/';
                }

                NormalizedPath = pathAsString[0] == '/' ? pathAsString : $"/{pathAsString}";
            }
            else
            {
                if (nativePath is not string pathAsString)
                {
                    throw new ArgumentException($"{nameof(nativePath)} is not a string.");
                }

                if (pathAsString[0] != '/')
                {
                    throw new ArgumentException($"{nameof(nativePath)} is an invalid path (doesn't start with '/').");
                }

                if (isDirectory && !pathAsString.EndsWith('/'))
                {
                    pathAsString += '/';
                }

                NormalizedPath = pathAsString;
            }
        }

        /// <summary>
        /// Returns a new FilePath representing an empty path.
        /// </summary>
        public static FilePath Empty => new("");

        public Uri ToUri()
        {
            return new Uri(new Uri("file://"), NormalizedPath);
        }

        /// <summary>
        /// Returns true if the path is a directory path, false otherwise. The path is a directory if
        /// <see cref="NormalizedPath"/> ends with an "/".
        /// </summary>
        /// <returns>True if the path is a directory path, false otherwise</returns>
        public bool IsDirectory()
        {
            return NormalizedPath[^1] == '/';
        }

        public override string ToString()
        {
            return NormalizedPath;
        }

        /// <summary>
        /// Checks whether the given path is valid on the runtime platform or not. Note that the validity is not
        /// guaranteed.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>True if the path is valid, false otherwise. There might be false-positives.</returns>
        public static bool IsPathValid(string path)
        {
            //check path
            int invalidIndex = path.IndexOfAny(Path.GetInvalidPathChars());
            if (invalidIndex >= 0)
            {
                return false;
            }

            ReadOnlySpan<char> fileName = path.AsSpan(path.LastIndexOf('/') + 1);

            //check file name
            invalidIndex = fileName.IndexOfAny(Path.GetInvalidFileNameChars());
            if (invalidIndex >= 0)
            {
                return false;
            }

            //taken from https://stackoverflow.com/a/62888/23361865
            if (OperatingSystem.IsWindows())
            {
                if (fileName.Contains(
                        new ReadOnlySpan<char>(['<', '>', ':', '"', '/', '\\', '|', '?', '*']),
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                //if there are characters in the ASCII 0-31 range, OR if the file name is all periods
                bool isAllPeriods = true;
                foreach (char c in fileName)
                {
                    if (c <= 31)
                    {
                        return false;
                    }

                    if (c != '.')
                    {
                        isAllPeriods = false;
                    }
                }

                if (isAllPeriods)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
