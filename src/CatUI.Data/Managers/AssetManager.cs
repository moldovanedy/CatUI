using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CatUI.Data.Assets;

namespace CatUI.Data.Managers;

/// <summary>
/// Handles asset loading, caching, and saving. Supports both loading from assemblies (embedded resources in the app)
/// and loading from asset files (files (usually *.dat) created at compile-time using CatUIUtility or even ar
/// run-time).
/// </summary>
/// <remarks>
/// If cached, the assets must have different paths. If a duplicate path is found, the later asset won't be
/// cached.
/// </remarks>
public static class AssetsManager
{
    private static readonly Dictionary<string, Asset> _cachedAssets = new();

    private static readonly List<Assembly> _assemblies = [];

    /// <summary>
    /// Shorthand for using LoadFromAssembly or LoadFromFile. It searches the cache, if nothing was found it calls
    /// <see cref="LoadFromAssembly{T}(string,bool)"/>, then if nothing was found it calls
    /// <see cref="LoadFromFileAsync{T}"/> and returns its result.
    /// </summary>
    /// <remarks>
    /// For assemblies, the assembly must be added first as an asset assembly (using <see cref="AddAssetAssembly"/>),
    /// otherwise the loading will fail for assemblies and go to the next modality: asset files.
    /// </remarks>
    /// <param name="path">The relative path of the asset.</param>
    /// <param name="shouldCache">If true, the asset is cached for faster loading on later calls.</param>
    /// <typeparam name="T">The type of the desired asset.</typeparam>
    /// <returns>The desired asset or null if one wasn't found.</returns>
    public static T Load<T>(string path, bool shouldCache = true) where T : Asset, new()
    {
        if (_cachedAssets.TryGetValue(path, out Asset? asset))
        {
            return (T)asset;
        }

        var finalAsset = LoadFromAssembly<T>(path, shouldCache);
        if (finalAsset != null)
        {
            return finalAsset;
        }

        finalAsset = LoadFromFileAsync<T>(path, shouldCache).GetAwaiter().GetResult();
        return finalAsset;
    }

    /// <summary>
    /// Async version of <see cref="Load{T}"/>.
    /// </summary>
    /// <inheritdoc cref="Load{T}"/>
    public static async Task<T?> LoadAsync<T>(string path, bool shouldCache = true) where T : Asset, new()
    {
        if (_cachedAssets.TryGetValue(path, out Asset? asset))
        {
            return (T)asset;
        }

        var finalAsset = await LoadFromAssemblyAsync<T>(path, shouldCache);
        if (finalAsset != null)
        {
            return finalAsset;
        }

        finalAsset = await LoadFromFileAsync<T>(path, shouldCache);
        return finalAsset;
    }

    /// <summary>
    /// Marks the given assembly as an "asset assembly", meaning that the methods specific for asset loading from
    /// assembly will look into this assembly for the specified asset (those assemblies are kept in a list to make
    /// loading faster).
    /// </summary>
    /// <param name="assembly">
    /// The assembly to add. See the static methods of <see cref="Assembly"/> for more information.
    /// </param>
    /// <returns>True if the method succeeded, false otherwise.</returns>
    public static bool AddAssetAssembly(Assembly assembly)
    {
        if (_assemblies.Contains(assembly))
        {
            return false;
        }

        _assemblies.Add(assembly);
        CatLogger.LogVerbose($"Added asset assembly \"{assembly.FullName}\".");
        return true;
    }

    /// <summary>
    /// Loads an asset from the assembly of the given type, specified by the asset path that is always relative
    /// to the root directory (the directory where the .csproj file is located, so for a directory named "Assets"
    /// the path would start with "/Assets/").
    /// All the files must be set as "Embedded resource" to be retrievable.
    /// </summary>
    /// <typeparam name="T">The type of asset desired.</typeparam>
    /// <param name="assetPath">
    /// The path of the assembly, relative to the directory where the .csproj is located.
    /// </param>
    /// <param name="classFromAssembly">
    /// A class type from the desired assembly. This will try to get the assembly by using
    /// <see cref="Assembly.GetAssembly(Type)"/> on the given type.
    /// </param>
    /// <param name="shouldCache">
    /// If true, will hold a reference to the asset internally, so later calls will return the asset much faster.
    /// </param>
    /// <returns>The asset from the specified path if one was found, null otherwise.</returns>
    public static T? LoadFromAssembly<T>(string assetPath, Type classFromAssembly, bool shouldCache = true)
        where T : Asset, new()
    {
        var mainAssembly = Assembly.GetAssembly(classFromAssembly);
        if (mainAssembly == null)
        {
            return null;
        }

        if (_cachedAssets.TryGetValue(assetPath, out Asset? asset))
        {
            return (T)asset;
        }

        assetPath = assetPath.Replace('/', '.');
        string asmName = mainAssembly.GetName().ToString();
        asmName = asmName.Split(',')[0];

        Stream? fs = mainAssembly.GetManifestResourceStream($"{asmName}{assetPath}");
        if (fs == null)
        {
            return null;
        }

        var finalAsset = new T();
        finalAsset.LoadFromStream(fs);
        if (shouldCache)
        {
            _cachedAssets.TryAdd(assetPath, finalAsset);
        }

        return finalAsset;
    }

    /// <summary>
    /// Async version of <see cref="LoadFromAssembly{T}(string, Type, bool)"/>.
    /// </summary>
    public static async Task<T?> LoadFromAssemblyAsync<T>(
        string assetPath,
        Type classFromAssembly,
        bool shouldCache = true)
        where T : Asset, new()
    {
        var mainAssembly = Assembly.GetAssembly(classFromAssembly);
        if (mainAssembly == null)
        {
            return null;
        }

        if (_cachedAssets.TryGetValue(assetPath, out Asset? asset))
        {
            return (T)asset;
        }

        assetPath = assetPath.Replace('/', '.');
        string asmName = mainAssembly.GetName().ToString();
        asmName = asmName.Split(',')[0];

        Stream? fs = mainAssembly.GetManifestResourceStream($"{asmName}{assetPath}");
        if (fs == null)
        {
            return null;
        }

        var finalAsset = new T();
        await finalAsset.LoadFromStreamAsync(fs);
        if (shouldCache)
        {
            _cachedAssets.TryAdd(assetPath, finalAsset);
        }

        return finalAsset;
    }

    /// <summary>
    /// Loads an asset from one of the loaded "asset assemblies", which can incur a small performance penalty
    /// when you have a lot of loaded asset assemblies (see <see cref="AddAssetAssembly(Assembly)"/>).
    /// All the files must be set as "Embedded resource" to be retrievable.
    /// The asset path is always relative to the root directory (the directory where the .csproj file is located, 
    /// so for a directory named "Assets" the path would start with "/Assets/").
    /// </summary>
    /// <typeparam name="T">The type of asset desired.</typeparam>
    /// <param name="assetPath">
    /// The path of the assembly, relative to the directory where the .csproj is located.
    /// </param>
    /// <param name="shouldCache">
    /// If true, will hold a reference to the asset internally, so later calls will return the asset much faster.
    /// </param>
    /// <returns>The asset from the specified path if one was found, null otherwise.</returns>
    public static T? LoadFromAssembly<T>(string assetPath, bool shouldCache = true) where T : Asset, new()
    {
        foreach (Assembly asm in _assemblies)
        {
            if (_cachedAssets.TryGetValue(assetPath, out Asset? asset))
            {
                return (T)asset;
            }

            assetPath = assetPath.Replace('/', '.');
            string asmName = asm.GetName().ToString();
            asmName = asmName.Split(',')[0];

            Stream? fs = asm.GetManifestResourceStream($"{asmName}{assetPath}");
            if (fs == null)
            {
                return null;
            }

            var finalAsset = new T();
            finalAsset.LoadFromStream(fs);
            if (shouldCache)
            {
                _cachedAssets.TryAdd(assetPath, finalAsset);
            }

            return finalAsset;
        }

        return null;
    }

    /// <summary>
    /// Async version of <see cref="LoadFromAssembly{T}(string, bool)"/>.
    /// </summary>
    public static async Task<T?> LoadFromAssemblyAsync<T>(string assetPath, bool shouldCache = true)
        where T : Asset, new()
    {
        foreach (Assembly asm in _assemblies)
        {
            if (_cachedAssets.TryGetValue(assetPath, out Asset? asset))
            {
                return (T)asset;
            }

            assetPath = assetPath.Replace('/', '.');
            string asmName = asm.GetName().ToString();
            asmName = asmName.Split(',')[0];

            Stream? fs = asm.GetManifestResourceStream($"{asmName}{assetPath}");
            if (fs == null)
            {
                return null;
            }

            var finalAsset = new T();
            await finalAsset.LoadFromStreamAsync(fs);
            if (shouldCache)
            {
                _cachedAssets.TryAdd(assetPath, finalAsset);
            }

            return finalAsset;
        }

        return null;
    }

    /// <summary>
    /// Loads an asset from the file system. This can be either the files that ship with the app (CopyToOutputDirectory)
    /// or an arbitrary file from the user's device (this is subject to restrictions, especially on mobile).
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are lots of restrictions regarding arbitrary file reads, so it's recommended to use it only for special
    /// known directories such as the app's execution directory or the data directory.
    /// </para>
    /// <para>
    /// On Windows, paths will be converted from "/" to "\", but the root directory still needs to have the drive letter
    /// without the colon in the path. This is only applied to global paths, loading assets from the execution directory
    /// does not have the drive letter problem.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of asset desired.</typeparam>
    /// <param name="path">
    /// The path of the assembly. If isGlobal is false, the path must begin with "/", pointing to the app's
    /// execution directory (generally the project directory); otherwise, if the path begins with "/", it's treated
    /// as an absolute path (beginning from the device root directory), otherwise as a relative path (beginning from
    /// the app's execution directory).
    /// </param>
    /// <param name="isGlobal">
    /// If true, it can reference files from the entire device file system. Otherwise (the default value), it can
    /// only reference files from the app's execution directory.
    /// </param>
    /// <param name="shouldCache">
    /// If true, will hold a reference to the asset internally, so later calls will return the asset much
    /// faster.
    /// </param>
    /// <returns>
    /// The task containing an asset from the specified path if one was found, a task containing null otherwise.
    /// </returns>
    public static async Task<T> LoadFromFileAsync<T>(string path, bool isGlobal = false, bool shouldCache = true)
        where T : Asset, new()
    {
        if (_cachedAssets.TryGetValue(path, out Asset? asset))
        {
            return (T)asset;
        }

        if (OperatingSystem.IsWindows())
        {
            path = path.Replace('/', '\\');
        }

        FileStream stream =
            isGlobal
                ? File.OpenRead(path)
                : File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + path);

        stream.Seek(0, SeekOrigin.End);
        long size = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);

        byte[] assetRawData = new byte[size];
        long bytesWritten = 0;

        byte[] buffer = new byte[4096];
        long position = stream.Position;
        while (position < size)
        {
            int limit = await stream.ReadAsync(buffer.AsMemory(0, 4096));
            Array.Copy(
                buffer,
                0,
                assetRawData,
                bytesWritten,
                limit);

            bytesWritten += limit;
            position += limit;
        }

        var finalAsset = new T();
        finalAsset.LoadFromRawData(assetRawData);
        if (shouldCache)
        {
            _cachedAssets.TryAdd(path, finalAsset);
        }

        return finalAsset;
    }


    /// <summary>
    /// Removes the asset from the cache at the given path.
    /// </summary>
    /// <param name="assetPath">The path to the asset.</param>
    /// <returns>True if the asset was cleared successfully, false otherwise.</returns>
    public static bool RemoveFromCache(string assetPath)
    {
        return _cachedAssets.Remove(assetPath);
    }

    /// <summary>
    /// Clears all the cached resources from the internal dictionary.
    /// </summary>
    public static void PurgeCache()
    {
        _cachedAssets.Clear();
    }
}
