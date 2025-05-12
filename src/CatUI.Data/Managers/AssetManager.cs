using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CatUI.Data.Assets;
using CatUI.Utils;

namespace CatUI.Data.Managers
{
    /// <summary>
    /// Handles asset loading, caching and saving. Supports both loading from assemblies (embedded resources in the app)
    /// and loading from asset files (files (usually *.dat) created at compile-time using CatUIUtility or even ar
    /// run-time).
    /// </summary>
    /// <remarks>
    /// If cached, the assets must have different paths. If a duplicate path is found, the later asset won't be
    /// cached.
    /// </remarks>
    public static class AssetsManager
    {
        /// <summary>
        /// Represents the number of loaded asset files (.dat) during the application lifetime.
        /// </summary>
        public static ushort NumberOfLoadedAssetFiles { get; private set; }

        private static readonly Dictionary<string, Asset> _cachedAssets = new();

        /// <summary>
        /// Represent the actual asset paths, the value is composed of most significant 2 bytes
        /// which are an index in the <see cref="_assetFilesPaths"/> and a 6 byte position of the
        /// asset data beginning in the file.
        /// </summary>
        private static readonly Dictionary<string, ulong> _assetPaths = new();

        private static readonly List<string> _assetFilesPaths = [];
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
        public static T? Load<T>(string path, bool shouldCache = true) where T : Asset, new()
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
            return true;
        }

        /// <summary>
        /// Loads an asset from the assembly of the given type, specified by the asset path that is always relative
        /// to the root directory (the directory where the .csproj file is located, so for a directory named "Assets"
        /// the path would start with "/Assets/").
        /// All the files must be set as "Embedded resource" to be retrievable.
        /// </summary>
        /// <remarks>
        /// You can also use .dat files for resources, together with methods like
        /// <see cref="LoadFromFileAsync{T}(string, bool)"/>, <see cref="LoadMetadataFromStreamAsync(Stream)"/> and
        /// <see cref="GetAssetFileStream"/> for asset handling.
        /// </remarks>
        /// <typeparam name="T">The type of asset desired.</typeparam>
        /// <param name="assetPath">
        /// The path of the assembly, relative to the directory where the .csproj is located.
        /// </param>
        /// <param name="classFromAssembly">
        /// A class type from the desired assembly. This will try to get the assembly by using
        /// <see cref="Assembly.GetAssembly(Type)"/> on the given type.
        /// </param>
        /// <param name="shouldCache">
        /// If true, will hold a reference to the asset internally, so subsequent calls will return the asset much
        /// faster.
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
        /// All the files must be set as "Embedded resource" in order to be retrievable.
        /// The asset path is always relative to the root directory (the directory where the .csproj file is located, 
        /// so for a directory named "Assets" the path would start with "/Assets/").
        /// </summary>
        /// <remarks>
        /// You can also use .dat files for resources, together with methods like
        /// <see cref="LoadFromFileAsync{T}(string, bool)"/>, <see cref="LoadMetadataFromStreamAsync(Stream)"/> and
        /// <see cref="GetAssetFileStream"/> for efficient asset handling.
        /// </remarks>
        /// <typeparam name="T">The type of asset desired.</typeparam>
        /// <param name="assetPath">
        /// The path of the assembly, relative to the directory where the .csproj is located.
        /// </param>
        /// <param name="shouldCache">
        /// If true, will hold a reference to the asset internally, so subsequent calls
        /// will return the asset much faster.
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
        /// Loads an asset from one of the loaded asset files, specified by the asset path that is always relative
        /// to the project root directory. An asset file containing the asset must be loaded before calling this method
        /// using <see cref="LoadMetadataFromFileAsync(string)"/> or<see cref="LoadMetadataFromStreamAsync(Stream)"/>.
        /// </summary>
        /// <remarks>
        /// To create asset files, you can use the CatUIUtility.
        /// </remarks>
        /// <typeparam name="T">The type of asset desired.</typeparam>
        /// <param name="path">
        /// The path of the assembly, always beginning with "/", pointing to the project root directory.
        /// </param>
        /// <param name="shouldCache">
        /// If true, will hold a reference to the asset internally, so subsequent calls will return the asset much
        /// faster.
        /// </param>
        /// <returns>
        /// The task containing an asset from the specified path if one was found, a task containing null otherwise.
        /// </returns>
        public static async Task<T?> LoadFromFileAsync<T>(string path, bool shouldCache = true) where T : Asset, new()
        {
            if (_cachedAssets.TryGetValue(path, out Asset? asset))
            {
                return (T)asset;
            }

            ObjectRef<long> endPositionRef = new();
            FileStream? stream = GetAssetFileStream(path, endPositionRef);
            if (stream == null)
            {
                return null;
            }

            byte[] assetRawData = new byte[endPositionRef.Value - stream.Position];
            long bytesWritten = 0;

            byte[] buffer = new byte[4096];
            long position = stream.Position;
            while (position < endPositionRef.Value)
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
        /// Returns a stream from one of the loaded asset files, specified by the asset path that is always relative
        /// to the project root directory. An asset file containing the asset must be loaded before calling this method
        /// using <see cref="LoadMetadataFromFileAsync(string)"/> or <see cref="LoadMetadataFromStreamAsync(Stream)"/>.
        /// </summary>
        /// <remarks>
        /// The stream has its position at the beginning of the asset data, while the endPosition will specify the 
        /// absolute byte position of the end of the asset data.
        /// In order to create asset files, you can use the CatUIUtility.
        /// </remarks>
        /// <param name="path">
        /// The path of the assembly, always beginning with "/", pointing to the project root directory.
        /// </param>
        /// <param name="endPosition">
        /// An <see cref="ObjectRef{T}"/> ref object whose <see cref="ObjectRef{T}.Value"/> will be set to
        /// the absolute byte position of the end of the asset data.
        /// </param>
        /// <returns>A FileStream configured as specified above if the asset was found, null otherwise.</returns>
        /// <exception cref="IOException">Thrown if it can't read the asset size.</exception>
        public static FileStream? GetAssetFileStream(string path, ObjectRef<long> endPosition)
        {
            if (!_assetPaths.TryGetValue(path, out ulong value))
            {
                return null;
            }

            int fileIndex = (int)(value >> 48);
            if (_assetFilesPaths.Count < fileIndex)
            {
                return null;
            }

            var fs = new FileStream(_assetFilesPaths[fileIndex], FileMode.Open, FileAccess.Read);
            long position = (long)(value & 0xff_ff_ff_ff_ff_ff);
            fs.Seek(position, SeekOrigin.Begin);

            byte[] assetSizeRaw = new byte[6];
            int bytesRead = fs.Read(assetSizeRaw, 0, 6);
            if (bytesRead != 6)
            {
                throw new IOException("Could not the read the asset file size.");
            }

            long assetSize = BinaryUtils.ConvertBytesToLong(assetSizeRaw, 0);

            endPosition.Value = position + 6 + assetSize;
            return fs;
        }

        /// <summary>
        /// Loads only the metadata of the assets contained in the asset file. It is necessary to load the metadata first,
        /// then load the actual assets. This will NOT load all the assets in memory, use the load functions for that.
        /// </summary>
        /// <remarks>
        /// If an asset's metadata with the same virtual path and name is already loaded, the method will throw an
        /// `EXCEPTION`. If this method throws an exception while loading the data, all the assets' metadata already
        /// loaded will remain loaded. While this method will work with very large files, consider splitting your
        /// assets into multiple files if you have a lot of them.
        /// </remarks>
        /// <param name="path">The path to an asset file.</param>
        public static async Task LoadMetadataFromFileAsync(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            await LoadMetadataFromStreamAsync(fs);
            _assetFilesPaths.Add(path);
        }

        /// <summary>
        /// Loads only the metadata of the assets contained in the asset file. It is necessary to load the metadata first,
        /// then load the actual assets. This will NOT load all the assets in memory, use the load functions for that.
        /// </summary>
        /// <remarks>
        /// If an asset's metadata with the same virtual path and name is already loaded, the method will throw an
        /// `EXCEPTION`. If this method throws an exception while loading the data, all the assets' metadata already
        /// loaded will remain loaded. While this method will work with very large files, consider splitting your
        /// assets into multiple files if you have a lot of them.
        /// </remarks>
        /// <param name="stream">The stream to an asset file.</param>
        /// <exception cref="IOException">Thrown if it can't read the asset size.</exception>
        public static async Task LoadMetadataFromStreamAsync(Stream stream)
        {
            if (stream.Length < 6)
            {
                throw new FormatException("Invalid format");
            }

            stream.Seek(6, SeekOrigin.End);
            byte[] dictionaryStartPosRaw = new byte[6];
            // ReSharper disable once MethodHasAsyncOverload
            int bytesRead = stream.Read(dictionaryStartPosRaw, 0, 6);
            if (bytesRead != 6)
            {
                throw new IOException("Could not the read the asset file length.");
            }

            //go to the dictionary start
            long pos = BinaryUtils.ConvertBytesToLong(dictionaryStartPosRaw, 0);
            stream.Seek(pos, SeekOrigin.Begin);

            byte[] buffer = new byte[4096];
            byte[]? assetPathRaw = null, assetPositionRaw = null;
            int assetPositionWrittenBytes = 0;
            while (pos < stream.Length)
            {
                int limit = await stream.ReadAsync(buffer.AsMemory(0, 4096));
                int bufferPos = 0;
                while (bufferPos < limit)
                {
                    if (assetPositionRaw != null)
                    {
                        for (int i = assetPositionWrittenBytes; i < 6; i++)
                        {
                            assetPositionRaw[i] = buffer[bufferPos];
                            bufferPos++;
                        }

                        if (assetPathRaw != null)
                        {
                            goto SaveAssetMetadata;
                        }
                        //if the path is null here, it must be an error
                        else
                        {
                            throw new FormatException("Invalid format");
                        }
                    }

                    if (assetPathRaw != null)
                    {
                        int newDimension = assetPathRaw.Length;
                        while (buffer[bufferPos] != 0 && bufferPos < limit)
                        {
                            newDimension++;
                            bufferPos++;
                        }

                        //if the limit was reached here, it must be an invalid format as a path can't have more than 4096 bytes
                        //and the only time a buffer is less than 4096 bytes is when EOF is reached,
                        //meaning the path is incomplete or at least is missing the position
                        if (bufferPos >= limit)
                        {
                            throw new FormatException("Invalid format");
                        }

                        //pass over the '\0'
                        bufferPos++;

                        byte[] newPathRaw = new byte[newDimension];
                        //copy old portion
                        Array.Copy(assetPathRaw, newPathRaw, assetPathRaw.Length);
                        //copy remaining portion
                        Array.Copy(buffer, 0, newPathRaw, assetPathRaw.Length, newDimension - assetPathRaw.Length);
                        //assign
                        assetPathRaw = newPathRaw;

                        //resolve the position
                        if (limit - bufferPos < 6)
                        {
                            assetPositionRaw = new byte[limit - bufferPos];
                            for (int i = 0; i < limit - bufferPos; i++)
                            {
                                assetPositionRaw[i] = buffer[bufferPos];
                                bufferPos++;
                            }

                            continue;
                        }

                        assetPositionRaw = new byte[6];
                        for (int i = 0; i < 6; i++)
                        {
                            assetPositionRaw[i] = buffer[bufferPos];
                            bufferPos++;
                        }

                        goto SaveAssetMetadata;
                    }

                    #region Path

                    int stringStart = bufferPos;
                    while (buffer[bufferPos] != 0 && bufferPos < limit)
                    {
                        bufferPos++;
                    }

                    //the end of buffer was reached, save the relevant portion of data and continue to the next buffer read
                    if (bufferPos >= limit)
                    {
                        assetPathRaw = new byte[stringStart - bufferPos];
                        for (int i = 0; i < assetPathRaw.Length; i++)
                        {
                            assetPathRaw[i] = buffer[stringStart + i];
                        }

                        continue;
                    }
                    //if the string is not empty
                    else if (stringStart != bufferPos)
                    {
                        //pass over the '\0'
                        bufferPos++;
                    }
                    //empty strings are not allowed as paths
                    else
                    {
                        throw new Exception("Invalid format");
                    }

                    assetPathRaw = new byte[bufferPos - stringStart - 1];
                    for (int i = 0; i < assetPathRaw.Length; i++)
                    {
                        assetPathRaw[i] = buffer[stringStart + i];
                    }

                    #endregion //Path

                    #region Position

                    if (limit - bufferPos < 6)
                    {
                        assetPositionRaw = new byte[limit - bufferPos];
                        for (int i = 0; i < limit - bufferPos; i++)
                        {
                            assetPositionRaw[i] = buffer[bufferPos];
                            bufferPos++;
                        }

                        continue;
                    }
                    else
                    {
                        assetPositionRaw = new byte[6];
                        for (int i = 0; i < 6; i++)
                        {
                            assetPositionRaw[i] = buffer[bufferPos];
                            bufferPos++;
                        }
                    }

                    #endregion //Position

                SaveAssetMetadata:
                    ulong savedValue = (ulong)NumberOfLoadedAssetFiles << 48;
                    savedValue |= (ulong)BinaryUtils.ConvertBytesToLong(assetPositionRaw, 0) & 0xff_ff_ff_ff_ff_ff;

                    _assetPaths.Add(
                        Encoding.UTF8.GetString(assetPathRaw),
                        savedValue);
                    assetPathRaw = null;
                    assetPositionRaw = null;
                }

                pos += limit;
            }

            NumberOfLoadedAssetFiles++;
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
}
