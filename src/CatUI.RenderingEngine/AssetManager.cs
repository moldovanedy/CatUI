using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Data.Assets;
using CatUI.Shared;
using Game.Shared.Utils;

namespace CatUI.RenderingEngine
{
    public static class AssetsManager
    {
        /// <summary>
        /// Represents the number of loaded asset files (.dat) during the application lifetime.
        /// </summary>
        public static ushort NumberOfLoadedAssetFiles { get; private set; }

        private static Dictionary<string, Asset> CachedAssets { get; } = new Dictionary<string, Asset>();
        /// <summary>
        /// Represent the actual asset paths, the value is composed of most significant 2 bytes
        /// which are an index in the <see cref="AssetFilesPaths"/> and a 6 byte position of the
        /// asset data beginning in the file.
        /// </summary>
        private static Dictionary<string, ulong> AssetPaths { get; } = new Dictionary<string, ulong>();
        private static List<string> AssetFilesPaths { get; } = new List<string>();

        /// <summary>
        /// Loads an asset from the given assembly, specified by the asset path that is always relative to the "Assets" folder.
        /// All the files must be set as "Embedded resource" in order to be retrievable.
        /// </summary>
        /// <remarks>
        /// Because of the complexity of setting each file as "Embedded resource" and getting the right assembly to load assets from,
        /// it is recommended to use .dat files for resources, together with methods like
        /// <see cref="LoadAsync{T}(string, bool)"/>, <see cref="LoadMetadataFromStreamAsync(Stream)"/> and <see cref="GetAssetFileStream(string, AsyncRef{long})"/>
        /// for efficient asset handling.
        /// </remarks>
        /// <typeparam name="T">The type of asset desired.</typeparam>
        /// <param name="mainAssembly">
        /// A reference to the assembly that holds the resources, usually retrieved from
        /// <see cref="Assembly.GetExecutingAssembly()"/>, but this is not always the case.
        /// </param>
        /// <param name="assetPath">The path of the assembly, always beginning with "/Assets"</param>
        /// <param name="shouldCache">
        /// If true, will hold a reference to the asset internally, so subsequent calls
        /// will return the asset much faster.
        /// </param>
        /// <returns>The asset from the specified path if one was found, null otherwise.</returns>
        public static T? LoadFromAssembly<T>(Assembly mainAssembly, string assetPath, bool shouldCache = true) where T : Asset, new()
        {
            if (CachedAssets.TryGetValue(assetPath, out Asset? asset))
            {
                return (T)asset;
            }

            assetPath = assetPath.Replace('/', '.');
            string asmName = mainAssembly.GetName().ToString();
            asmName = asmName.Split(',')[0];

            Stream? fs =
                mainAssembly
                    .GetManifestResourceStream($"{asmName}{assetPath}");
            if (fs == null)
            {
                return null;
            }

            T finalAsset = new T();
            finalAsset.LoadFromRawData(fs);
            if (shouldCache)
            {
                CachedAssets.Add(assetPath, finalAsset);
            }

            return finalAsset;
        }

        /// <summary>
        /// Loads an asset from one of the loaded asset files, specified by the asset path that is always relative
        /// to the "Assets" folder. An asset file containing the asset must be loaded before calling this method using
        /// <see cref="LoadMetadataFromFileAsync(string)"/> or <see cref="LoadMetadataFromStreamAsync(Stream)"/>.
        /// </summary>
        /// <remarks>
        /// In order to create asset files, you can use the Cat DevTool.
        /// </remarks>
        /// <typeparam name="T">The type of asset desired.</typeparam>
        /// <param name="assetPath">The path of the assembly, always beginning with "/Assets"</param>
        /// <param name="shouldCache">
        /// If true, will hold a reference to the asset internally, so subsequent calls
        /// will return the asset much faster.
        /// </param>
        /// <returns>The task containing an asset from the specified path if one was found, a task containing null otherwise.</returns>
        public static async Task<T?> LoadAsync<T>(string path, bool shouldCache = true) where T : Asset, new()
        {
            if (CachedAssets.TryGetValue(path, out Asset? asset))
            {
                return (T)asset;
            }

            AsyncRef<long> endPositionRef = new AsyncRef<long>();
            FileStream? stream = GetAssetFileStream(path, endPositionRef);
            if (stream == null)
            {
                return null;
            }

            byte[] assetRawData = new byte[endPositionRef.Ref - stream.Position];
            long bytesWritten = 0;

            byte[] buffer = new byte[4096];
            long position = stream.Position;
            while (position < endPositionRef.Ref)
            {
                int limit = await stream.ReadAsync(buffer.AsMemory(0, 4096));
                Array.Copy(buffer, 0, assetRawData, bytesWritten, limit);

                bytesWritten += limit;
                position += limit;
            }

            T finalAsset = new T();
            finalAsset.LoadFromRawData(assetRawData);
            if (shouldCache)
            {
                CachedAssets.Add(path, finalAsset);
            }
            return finalAsset;
        }

        /// <summary>
        /// Returns a stream from one of the loaded asset files, specified by the asset path that is always relative
        /// to the "Assets" folder. An asset file containing the asset must be loaded before calling this method using
        /// <see cref="LoadMetadataFromFileAsync(string)"/> or <see cref="LoadMetadataFromStreamAsync(Stream)"/>.
        /// </summary>
        /// <remarks>
        /// The stream has its position at the beginning of the asset data, while the endPosition will specify the 
        /// absolute byte position of the end of the asset data.
        /// In order to create asset files, you can use the Cat DevTool.
        /// </remarks>
        /// <typeparam name="T">The type of asset desired.</typeparam>
        /// <param name="assetPath">The path of the assembly, always beginning with "/Assets"</param>
        /// <param name="endPosition">
        /// An <see cref="AsyncRef{T}"/> ref object whose <see cref="AsyncRef{T}.Ref"/> will be set to
        /// the absolute byte position of the end of the asset data.
        /// </param>
        /// <returns>A FileStream configured as specified above if the asset was found, null otherwise.</returns>
        public static FileStream? GetAssetFileStream(string path, AsyncRef<long> endPosition)
        {
            if (!AssetPaths.TryGetValue(path, out ulong value))
            {
                return null;
            }

            int fileIndex = (int)(value >> 48);
            if (AssetFilesPaths.Count < fileIndex)
            {
                return null;
            }

            FileStream fs = new FileStream(AssetFilesPaths[fileIndex], FileMode.Open, FileAccess.Read);
            long position = (long)(value & 0xff_ff_ff_ff_ff_ff);
            fs.Seek(position, SeekOrigin.Begin);

            byte[] assetSizeRaw = new byte[6];
            fs.Read(assetSizeRaw, 0, 6);
            long assetSize = BinaryUtils.ConvertBytesToLong(assetSizeRaw, 0);

            endPosition.Ref = position + 6 + assetSize;
            return fs;
        }

        /// <summary>
        /// Loads only the metadata of the assets contained in the asset file. It is necessary to load the metadata first,
        /// then load the actual assets. This will NOT load all the assets in memory, use the load functions for that.
        /// </summary>
        /// <remarks>
        /// If an asset's metadata with the same virtual path and name is already loaded, the method will throw an `EXCEPTION`.
        /// If this method throws an exception while loading the data, all the assets' metadata already loaded will remain loaded.
        /// While this method will work with very large files, consider splitting your assets into multiple files if you have a lot of them.
        /// </remarks>
        /// <param name="path">The path to an asset file.</param>
        public static async Task LoadMetadataFromFileAsync(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            await LoadMetadataFromStreamAsync(fs);
        }

        /// <summary>
        /// Loads only the metadata of the assets contained in the asset file. It is necessary to load the metadata first,
        /// then load the actual assets. This will NOT load all the assets in memory, use the load functions for that.
        /// </summary>
        /// <remarks>
        /// If an asset's metadata with the same virtual path and name is already loaded, the method will throw an `EXCEPTION`.
        /// If this method throws an exception while loading the data, all the assets' metadata already loaded will remain loaded.
        /// While this method will work with very large files, consider splitting your assets into multiple files if you have a lot of them.
        /// </remarks>
        /// <param name="stream">The stream to an asset file.</param>
        public static async Task LoadMetadataFromStreamAsync(Stream stream)
        {
            if (stream.Length < 6)
            {
                throw new Exception("Invalid format");
            }

            stream.Seek(6, SeekOrigin.End);
            byte[] dictionaryStartPosRaw = new byte[6];
            stream.Read(dictionaryStartPosRaw, 0, 6);

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
                            throw new Exception("Invalid format");
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
                            throw new Exception("Invalid format");
                        }
                        else
                        {
                            //pass over the '\0'
                            bufferPos++;
                        }

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
                        else
                        {
                            assetPositionRaw = new byte[6];
                            for (int i = 0; i < 6; i++)
                            {
                                assetPositionRaw[i] = buffer[bufferPos];
                                bufferPos++;
                            }
                            goto SaveAssetMetadata;
                        }
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

                    AssetPaths.Add(
                        Encoding.UTF8.GetString(assetPathRaw),
                        savedValue);
                    assetPathRaw = null;
                    assetPositionRaw = null;
                }

                pos += limit;
            }

            NumberOfLoadedAssetFiles++;
        }
    }
}
