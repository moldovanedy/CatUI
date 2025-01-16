using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatUI.Utils
{
    /// <summary>
    /// A useful utility that can read or write TOML-like files or byte streams, commonly used for storing settings.
    /// Allows primitive values, DateTime, string and null. Arrays are not supported.
    /// </summary>
    /// <remarks>Implements TOML version 0.1, but without arrays support.</remarks>
    public class SimpleSettingsHandler
    {
        private const int BUFFER_SIZE = 4096;

        /// <summary>
        /// Returns the root element of the settings hierarchy (i.e. the one that won't have "[...]" or a name).
        /// </summary>
        /// <returns></returns>
        public SectionNode Root
        {
            get
            {
                if (!_loadTask.IsCompleted)
                {
                    _loadTask.Wait();
                    _fs?.Close();
                }

                return _root;
            }
        }

        private SectionNode _root = null!;

        private readonly Task _loadTask;
        private readonly FileStream? _fs;
        private readonly string? _filePath;

        private readonly char[] _rawChars = new char[BUFFER_SIZE];
        private readonly List<ReadOnlyMemory<char>> _lines = new();
        private string _incompleteLine = "";

        private SectionNode? _currentSection;

        /// <summary>
        /// Creates an empty settings handler.
        /// </summary>
        public SimpleSettingsHandler()
        {
            _loadTask = Task.CompletedTask;
            _root = new SectionNode();
        }

        /// <summary>
        /// Loads the settings from the given file path. Immediately starts loading the settings, but will work
        /// asynchronously, so await <see cref="WaitForLoadingAsync"/> if you don't want to wait synchronously when
        /// doing the first operation.
        /// </summary>
        /// <param name="filePath">The path of the file to read. Must have at least read access.</param>
        /// <exception cref="IOException">Thrown if the file cannot be read because of permissions.</exception>
        public SimpleSettingsHandler(string filePath)
        {
            _filePath = filePath;
            _fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            _loadTask = ReadAllSettingsAsync(_fs);
        }

        /// <summary>
        /// Loads the settings from the given Stream. Immediately starts loading the settings, but will work
        /// asynchronously, so await <see cref="WaitForLoadingAsync"/> if you don't want to wait synchronously when
        /// doing the first operation.
        /// </summary>
        /// <remarks>Does NOT close the stream.</remarks>
        /// <param name="stream">The stream to read data from.</param>
        /// <exception cref="IOException">Thrown if the stream cannot be read (i.e. Stream.CanRead is false)</exception>
        public SimpleSettingsHandler(Stream stream)
        {
            _loadTask = ReadAllSettingsAsync(stream);
        }

        /// <summary>
        /// Asynchronously waits for the utility to read all data and create the in-memory structure.
        /// You MUST wait for completion. If you are in a non-async method, use Wait or something similar.
        /// </summary>
        /// <remarks>
        /// If you don't wait for completion, the next call of any function from this object will wait automatically
        /// so it doesn't create problems.
        /// </remarks>
        public async Task WaitForLoadingAsync()
        {
            await _loadTask;
            _fs?.Close();
        }

        #region Read

        private async Task ReadAllSettingsAsync(Stream fs)
        {
            _root = new SectionNode();
            if (!fs.CanRead)
            {
                throw new IOException("Cannot read settings stream.");
            }

            int readSize;
            byte[] buffer = new byte[BUFFER_SIZE];

            while ((readSize = await fs.ReadAsync(buffer)) != 0)
            {
                _lines.Clear();

                Decoder decoder = Encoding.UTF8.GetDecoder();
                int numberOfChars = decoder.GetCharCount(buffer, 0, readSize);

                int decodedCharCount = decoder.GetChars(
                    buffer, 0, readSize, _rawChars, 0, false);
                if (numberOfChars != decodedCharCount)
                {
                    throw new Exception("Invalid number of characters detected.");
                    return;
                }

                bool hasTreatedIncompleteLine = false;
                int rowStart = 0;
                for (int i = 0; i < numberOfChars; i++)
                {
                    if (_rawChars[i] == '\r' || _rawChars[i] == '\n')
                    {
                        //ignore blank lines or the LF after CR
                        if (rowStart == i)
                        {
                            rowStart++;
                            continue;
                        }

                        //this is only for the first row
                        if (_incompleteLine.Length > 0 && !hasTreatedIncompleteLine)
                        {
                            _incompleteLine += new string(_rawChars, rowStart, i - rowStart);
                            _lines.Add(_incompleteLine.ToCharArray());
                            _incompleteLine = "";

                            hasTreatedIncompleteLine = true;
                            rowStart = i + 1;
                            continue;
                        }

                        _lines.Add(new Memory<char>(_rawChars, rowStart, i - rowStart));
                        rowStart = i + 1;
                    }
                }

                for (int i = 0; i < _lines.Count; i++)
                {
                    InterpretLine(i);
                }

                if (rowStart != numberOfChars)
                {
                    //here it's ok to concatenate strings because it happens rarely (once every 4KiB)
                    _incompleteLine += new string(_rawChars, rowStart, numberOfChars - rowStart);
                }
            }
        }

        private void InterpretLine(int index)
        {
            ReadOnlySpan<char> row = _lines[index].Span;

            switch (row[0])
            {
                case '#':
                    return;
                case '[':
                    InterpretSectionDefinition(row);
                    break;
                default:
                    InterpretKeyValuePair(row);
                    break;
            }
        }

        private void InterpretSectionDefinition(ReadOnlySpan<char> row)
        {
            //reset section
            _currentSection = _root;

            if (row.Length < 3)
            {
                throw new FormatException($"Invalid section definition (too short): {row}");
            }

            int closingBracketIndex = row.IndexOf(']');
            if (closingBracketIndex == -1)
            {
                throw new FormatException($"Invalid section definition (no closing bracket): {row}");
            }

            string[] sections = row.Slice(1, closingBracketIndex - 1).ToString().Split('.');
            foreach (string section in sections)
            {
                SectionNode? existingSection = _currentSection.GetSection(section);
                if (existingSection != null)
                {
                    _currentSection = existingSection;
                    continue;
                }

                _currentSection = _currentSection?.AddEmptySection(section);
                if (_currentSection == null)
                {
                    throw new Exception($"Couldn't add section: {section}");
                }
            }
        }

        private void InterpretKeyValuePair(ReadOnlySpan<char> row)
        {
            int keyEndIndex = row.IndexOf('=');
            if (keyEndIndex == -1)
            {
                throw new FormatException($"Invalid entry definition (no assignment found): {row}");
            }

            int valueEndIndex = row.IndexOf('#') == -1 ? row.Length - 1 : row.IndexOf('#');

            string key = row[..keyEndIndex].ToString().Trim();
            string rawValue = row[(keyEndIndex + 1)..(valueEndIndex + 1)].ToString().Trim();
            object value = InterpretValue(rawValue);

            if (_currentSection == null)
            {
                _root[key] = value;
            }
            else
            {
                _currentSection[key] = value;
            }
        }

        private static object InterpretValue(string value)
        {
            if (value.Length < 1)
            {
                throw new FormatException($"Invalid value definition (no assignment found)");
            }

            if (value[0] == '"')
            {
                if (value.Length < 2)
                {
                    throw new FormatException($"Invalid string value definition (no closing quotes)");
                }

                int stringEndIndex = value.LastIndexOf('"');
                if (stringEndIndex == -1)
                {
                    throw new FormatException($"Invalid string value definition (no closing quotes): {value}");
                }

                StringBuilder sb = new();
                int i = 1;
                while (i < stringEndIndex)
                {
                    if (value[i] == '\\')
                    {
                        i++;
                        if (i == stringEndIndex)
                        {
                            throw new FormatException(
                                $"Invalid string value definition (invalid escape sequence): {value}");
                        }

                        switch (value[i])
                        {
                            case 'b':
                                sb.Append('\b');
                                break;
                            case 't':
                                sb.Append('\t');
                                break;
                            case 'n':
                                sb.Append('\n');
                                break;
                            case 'f':
                                sb.Append('\f');
                                break;
                            case 'r':
                                sb.Append('\r');
                                break;
                            case '"':
                                sb.Append('"');
                                break;
                            case '/':
                                sb.Append('/');
                                break;
                            case '\\':
                                sb.Append('\\');
                                break;
                            case 'u':
                                {
                                    if (i + 4 >= stringEndIndex)
                                    {
                                        throw new FormatException(
                                            $"Invalid string value definition (invalid Unicode escape sequence): {value}");
                                    }

                                    string unicodeCodePoint = value.Substring(i + 1, 4);
                                    if (!int.TryParse(
                                            unicodeCodePoint,
                                            NumberStyles.HexNumber,
                                            CultureInfo.CurrentCulture,
                                            out int codePoint))
                                    {
                                        throw new FormatException(
                                            $"Invalid string value definition (invalid Unicode code point): {value}");
                                    }

                                    sb.Append((char)codePoint);
                                    break;
                                }
                            default:
                                throw new FormatException(
                                    $"Invalid string value definition (unrecognized escape sequence): {value}");
                        }
                    }

                    sb.Append(value[i]);
                    i++;
                }

                return sb.ToString();
            }
            else if (char.IsDigit(value[0]) || value[0] == '-' || value[0] == '+')
            {
                if (value.Contains('.'))
                {
                    if (double.TryParse(value, out double d))
                    {
                        return d;
                    }

                    throw new FormatException($"Invalid floating-point (double) value: {value}");
                }
                else if (value.EndsWith('Z'))
                {
                    if (DateTime.TryParse(value, out DateTime dateTime))
                    {
                        return dateTime;
                    }

                    throw new FormatException($"Invalid DateTime (DateTime) value: {value}");
                }
                else
                {
                    if (long.TryParse(value, out long d))
                    {
                        return d;
                    }

                    throw new FormatException($"Invalid integer (long) value: {value}");
                }
            }
            else if (value[0] == 't')
            {
                if (value.Length < 4)
                {
                    throw new FormatException($"Invalid boolean value definition (unknown boolean value): {value})");
                }

                if (value.Substring(0, 4) != "true")
                {
                    throw new FormatException($"Invalid boolean value definition (unknown boolean value): {value}");
                }

                return true;
            }
            else if (value[0] == 'f')
            {
                if (value.Length < 5)
                {
                    throw new FormatException($"Invalid boolean value definition (unknown boolean value): {value})");
                }

                if (value.Substring(0, 5) != "false")
                {
                    throw new FormatException($"Invalid boolean value definition (unknown boolean value): {value}");
                }

                return false;
            }
            else
            {
                throw new FormatException($"Invalid value (unrecognized): {value}");
            }
        }

        #endregion //Read

        #region Write

        /// <summary>
        /// Overwrites the contents of the file from where the settings were read. Does not handle I/O exceptions.
        /// Only works if this object was created from a file (see <see cref="SimpleSettingsHandler(string)"/>).
        /// </summary>
        /// <exception cref="InvalidOperationException">If this object wasn't created from a file.</exception>
        public void SaveToFileWithOverwrite()
        {
            if (_filePath == null)
            {
                throw new InvalidOperationException("The settings utility was not initialized from a file.");
            }

            FileStream fs = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write);
            WriteAllSettingsAsync(fs).Wait();
            fs.Close();
        }

        /// <summary>
        /// Saves the current settings to a file to the given path (will be created if it doesn't exist).
        /// Does not handle I/O exceptions.
        /// </summary>
        /// <param name="filePath">The path to write to.</param>
        public void SaveToFile(string filePath)
        {
            FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            WriteAllSettingsAsync(fs).Wait();
            fs.Close();
        }

        /// <summary>
        /// Saves the current settings to the given stream (must have write permission).
        /// Does not handle stream write exceptions. Does not close the stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void SaveToStream(Stream stream)
        {
            WriteAllSettingsAsync(stream).Wait();
        }

        /// <inheritdoc cref="SaveToStream"/>
        /// <summary>
        /// Async version of <see cref="SaveToStream"/>
        /// </summary>
        public async Task SaveToStreamAsync(Stream stream)
        {
            await WriteAllSettingsAsync(stream);
        }

        private async Task WriteAllSettingsAsync(Stream stream)
        {
            await WriteSectionAsync(stream, _root);
        }

        private async Task WriteSectionAsync(Stream stream, SectionNode section)
        {
            if (section.ValuesCount > 0)
            {
                if (section != _root)
                {
                    StringBuilder sectionSb = new();
                    sectionSb.Append("\n[");

                    SectionNode? currentSection = section;
                    while (currentSection != null && currentSection != _root)
                    {
                        sectionSb.Insert(2, currentSection.Name);
                        currentSection = currentSection.Parent;

                        if (currentSection != null && currentSection != _root)
                        {
                            sectionSb.Insert(2, '.');
                        }
                    }

                    sectionSb.Append("]\n");
                    stream.Write(Encoding.UTF8.GetBytes(sectionSb.ToString()));
                }

                MemoryStream memoryStream = new();
                int thisChunkBytes = 0;
                bool hasWrittenChunk = false;

                foreach (KeyValuePair<string, object?> value in section.GetEntriesClone())
                {
                    WriteKeyValuePair(memoryStream, value, out int bytesWritten);
                    thisChunkBytes += bytesWritten;
                    hasWrittenChunk = false;

                    if (thisChunkBytes > BUFFER_SIZE)
                    {
                        memoryStream.Position = 0;
                        await memoryStream.CopyToAsync(stream, (int)memoryStream.Length);
                        memoryStream.SetLength(0);
                        thisChunkBytes = 0;
                        hasWrittenChunk = true;
                    }
                }

                if (!hasWrittenChunk)
                {
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(stream, (int)memoryStream.Length);
                    memoryStream.SetLength(0);
                }
            }

            foreach (SectionNode? subSection in section.GetSubSections())
            {
                await WriteSectionAsync(stream, subSection);
            }
        }

        private static void WriteKeyValuePair(
            MemoryStream memoryStream,
            KeyValuePair<string, object?> entry,
            out int bytesWritten)
        {
            bytesWritten = 0;
            byte[] rawValue;

            byte[] rawKey = Encoding.UTF8.GetBytes(entry.Key);
            memoryStream.Write(rawKey);
            memoryStream.WriteByte((byte)'=');
            bytesWritten += rawKey.Length;

            switch (entry.Value)
            {
                case null:
                    memoryStream.Write(new[] { (byte)'"', (byte)'"' });
                    bytesWritten += 2;
                    return;
                case string stringValue:
                    {
                        StringBuilder sb = new();
                        sb.Append('"');
                        foreach (char c in stringValue)
                        {
                            switch (c)
                            {
                                case '\b':
                                    sb.Append("\\b");
                                    break;
                                case '\t':
                                    sb.Append("\\t");
                                    break;
                                case '\n':
                                    sb.Append("\\n");
                                    break;
                                case '\f':
                                    sb.Append("\\f");
                                    break;
                                case '\r':
                                    sb.Append("\\r");
                                    break;
                                case '\"':
                                    sb.Append("\\\"");
                                    break;
                                case '/':
                                    sb.Append("\\/");
                                    break;
                                case '\\':
                                    sb.Append("\\\\");
                                    break;
                                default:
                                    if (c <= 0x1F)
                                    {
                                        sb.Append($"\\u{(int)c:X4}");
                                    }
                                    else
                                    {
                                        sb.Append(c);
                                    }

                                    break;
                            }
                        }

                        sb.Append('"');
                        rawValue = Encoding.UTF8.GetBytes(sb.ToString());
                        break;
                    }
                case byte byteValue:
                    rawValue = Encoding.UTF8.GetBytes(byteValue.ToString());
                    break;
                case sbyte sbyteValue:
                    rawValue = Encoding.UTF8.GetBytes(sbyteValue.ToString());
                    break;
                case short shortValue:
                    rawValue = Encoding.UTF8.GetBytes(shortValue.ToString());
                    break;
                case ushort ushortValue:
                    rawValue = Encoding.UTF8.GetBytes(ushortValue.ToString());
                    break;
                case int intValue:
                    rawValue = Encoding.UTF8.GetBytes(intValue.ToString());
                    break;
                case uint uintValue:
                    rawValue = Encoding.UTF8.GetBytes(uintValue.ToString());
                    break;
                case long longValue:
                    rawValue = Encoding.UTF8.GetBytes(longValue.ToString());
                    break;
                case ulong ulongValue when ulongValue > long.MaxValue:
                    throw new InvalidDataException($"Value is too high (larger than ulong.MaxValue): {ulongValue}");
                case ulong ulongValue:
                    rawValue = Encoding.UTF8.GetBytes(ulongValue.ToString());
                    break;
                case float floatValue:
                    rawValue = Encoding.UTF8.GetBytes(floatValue.ToString(CultureInfo.InvariantCulture));
                    break;
                case double doubleValue:
                    rawValue = Encoding.UTF8.GetBytes(doubleValue.ToString(CultureInfo.InvariantCulture));
                    break;
                case decimal decimalValue:
                    //TODO: log loss of precision if it is decimal
                    rawValue = Encoding.UTF8.GetBytes(decimalValue.ToString(CultureInfo.InvariantCulture));
                    break;
                case bool booleanValue:
                    rawValue = Encoding.UTF8.GetBytes(booleanValue ? "true" : "false");
                    break;
                case DateTime dateTimeValue:
                    rawValue = Encoding.UTF8.GetBytes(dateTimeValue.ToString("u"));
                    break;
                default:
                    throw new FormatException($"Invalid value type: {entry.Value?.GetType()}");
            }

            memoryStream.Write(rawValue);
            memoryStream.WriteByte(10);
            bytesWritten += rawValue.Length + 1;
        }

        #endregion

        #region Sections

        /// <summary>
        /// Searches the settings for the given section.
        /// </summary>
        /// <param name="path">It MUST use "/" as separator and contain a leading "/".</param>
        /// <returns> The section on the given path or null if it can't find a section.</returns>
        public SectionNode? GetSectionByPath(string path)
        {
            if (!_loadTask.IsCompleted)
            {
                _loadTask.Wait();
                _fs?.Close();
            }

            string[] sectionNames = path.Split('/');
            if (sectionNames.Length < 1)
            {
                return null;
            }

            int level = 1;
            SectionNode? currentSection = _root;
            while (level < sectionNames.Length)
            {
                currentSection = currentSection.GetSection(sectionNames[level]);
                if (currentSection == null)
                {
                    return null;
                }

                level++;
            }

            return currentSection;
        }

        #endregion //Sections

        /// <summary>
        /// Represents a section in the settings hierarchy.
        /// </summary>
        public class SectionNode
        {
            /// <summary>
            /// Gets or sets the section's name.
            /// </summary>
            public string Name
            {
                get => _name;
                set
                {
                    Parent?._childSections.Remove(_name);
                    _name = value;
                    Parent?._childSections.Add(_name, this);
                }
            }

            private string _name = "";

            /// <summary>
            /// The parent section. Null for root section.
            /// </summary>
            internal SectionNode? Parent { get; private set; }

            /// <summary>
            /// The number of values/entries in this section.
            /// </summary>
            public int ValuesCount => _values.Count;

            /// <summary>
            /// The number of subsections in this section.
            /// </summary>
            public int SectionsCount => _childSections.Count;

            private readonly Dictionary<string, SectionNode> _childSections = new();
            private readonly Dictionary<string, object?> _values = new();

            /// <summary>
            /// Adds the given section to this section. Fails if the given section already has a parent
            /// or if this section already has another child section with the same name. 
            /// </summary>
            /// <param name="newSection"></param>
            /// <returns>True if it succeeds, false otherwise.</returns>
            public bool AddSection(SectionNode newSection)
            {
                if (newSection.Parent != null)
                {
                    return false;
                }

                //overwriting a value is not permitted
                if (_values.ContainsKey(newSection.Name))
                {
                    return false;
                }

                if (!_childSections.TryAdd(newSection.Name, newSection))
                {
                    return false;
                }

                newSection.Parent = this;
                return true;
            }

            /// <summary>
            /// Adds a section with the given name to a parent and returns the section if it succeeded, null otherwise.
            /// </summary>
            /// <param name="name">The name for the new section.</param>
            /// <param name="parent">The parent section. If it's null, it will add the section to this section.</param>
            /// <returns>The new section if the addition succeeded, null otherwise.</returns>
            public SectionNode? AddEmptySection(string name, SectionNode? parent = null)
            {
                var newSection = new SectionNode() { Name = name };
                if (parent == null)
                {
                    return AddSection(newSection) ? newSection : null;
                }

                return parent.AddSection(newSection) ? newSection : null;
            }

            /// <summary>
            /// Returns the subsection with the given name or null if one wasn't found.
            /// </summary>
            /// <param name="name">The name of the subsection to search for.</param>
            /// <returns></returns>
            public SectionNode? GetSection(string name)
            {
                _childSections.TryGetValue(name, out SectionNode? section);
                return section;
            }

            /// <summary>
            /// Returns a list of all subsections of this section.
            /// </summary>
            /// <returns></returns>
            public List<SectionNode> GetSubSections()
            {
                return _childSections.Select(pair => pair.Value).ToList();
            }

            /// <summary>
            /// Removes the section with the given name if it exists.
            /// </summary>
            /// <param name="name">The name of the section to remove.</param>
            public void RemoveSection(string name)
            {
                if (!_childSections.TryGetValue(name, out SectionNode? section))
                {
                    return;
                }

                section.Parent = null;
                _childSections.Remove(name);
            }

            /// <summary>
            /// Removes all the subsections of this section, along with their values.
            /// </summary>
            public void RemoveAllSections()
            {
                while (_childSections.Count > 0)
                {
                    RemoveSection(_childSections.Keys.ElementAt(_childSections.Count - 1));
                }
            }

            /// <summary>
            /// Gets the value from this section by the given key or throws a KeyNotFoundException if the key isn't found.
            /// </summary>
            /// <param name="key">The key to search for.</param>
            /// <returns>The value searched for.</returns>
            /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
            public object? GetValue(string key)
            {
                if (!_values.TryGetValue(key, out object? value))
                {
                    throw new KeyNotFoundException($"Key \"{key}\" was not found in settings.");
                }

                return value;
            }

            /// <inheritdoc cref="GetValue"/>
            /// <typeparam name="T">The value type to cast to.</typeparam>
            public T? GetValue<T>(string key)
            {
                if (!_values.TryGetValue(key, out object? value))
                {
                    throw new KeyNotFoundException($"Key \"{key}\" was not found in settings.");
                }

                return (T?)value;
            }

            /// <summary>
            /// Tries to get a value from this section by the given key.
            /// </summary>
            /// <param name="key">The key to search for.</param>
            /// <param name="value">If the key is found, this will be the value, otherwise it will be default (T).</param>
            /// <typeparam name="T">The value type to cast to.</typeparam>
            /// <returns>True is the key is found, false otherwise.</returns>
            public bool TryGetValue<T>(string key, out T? value)
            {
                bool success = _values.TryGetValue(key, out object? rawValue);
                value = success ? (T)rawValue! : default;
                return success;
            }

            /// <summary>
            /// Tries to find a value by the given key path. If the path starts with a "/", it is an absolute path
            /// and the search start from the root section. If it starts with ".", the path is relative and the
            /// search starts from this section.
            /// </summary>
            /// <param name="path">The path of the key. Must start with "/" or ".".</param>
            /// <param name="value">If the key is found, this will be the value, otherwise it will be default (T).</param>
            /// <typeparam name="T">The value type to cast to.</typeparam>
            /// <returns>True is the key is found, false otherwise.</returns>
            /// <exception cref="ArgumentException">If the path starts with something other than "/" or ".".</exception>
            public bool TryGetValueByPath<T>(string path, out T? value)
            {
                value = default;
                if (string.IsNullOrWhiteSpace(path))
                {
                    return false;
                }

                bool isRelative = path.StartsWith('.');
                SectionNode? currentSection = this;
                if (!isRelative)
                {
                    if (!path.StartsWith('/'))
                    {
                        throw new ArgumentException("Path must start with '/' or '.'.", nameof(path));
                    }

                    //go to root
                    while (currentSection.Parent != null)
                    {
                        currentSection = currentSection.Parent;
                    }
                }

                string[] sections = path.Split('/');
                //first ("" or ".") and last (the key) are ignored
                for (int i = 1; i < sections.Length - 1; i++)
                {
                    currentSection = currentSection.GetSection(sections[i]);
                    if (currentSection == null)
                    {
                        return false;
                    }
                }

                return currentSection.TryGetValue(sections[^1], out value);
            }

            /// <summary>
            /// Returns a list of all the values.
            /// </summary>
            /// <returns></returns>
            public List<object?> GetValues()
            {
                return _values.Select(value => value.Value).ToList();
            }

            /// <summary>
            /// Returns a shallow clone of all the entries of this section. This means that changing the value
            /// will reflect in this section, but adding, removing or updating the key of the returned dictionary
            /// won't have any effect on this section.
            /// </summary>
            /// <returns></returns>
            public Dictionary<string, object?> GetEntriesClone()
            {
                return _values.Keys.ToDictionary(
                    key => key,
                    // ReSharper disable once ConvertClosureToMethodGroup
                    elementSelector => GetValue<object>(elementSelector));
            }

            public bool HasValue(string key)
            {
                return _values.ContainsKey(key);
            }

            public void SetValue(string key, object value)
            {
                _values[key] = value;
            }

            public void SetValue<T>(string key, T? value)
            {
                _values[key] = value;
            }

            public void RemoveValue(string key)
            {
                _values.Remove(key);
            }

            public void RemoveAllValues()
            {
                _values.Clear();
            }

            public object? this[string key]
            {
                get => _values[key];
                set => _values[key] = value;
            }
        }
    }
}
