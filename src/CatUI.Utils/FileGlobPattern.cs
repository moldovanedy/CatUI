using System.Text;

namespace CatUI.Utils;

public class FileGlobPattern
{
    private readonly string[] _patterns;

    /// <summary>
    /// Will create the glob-like pattern from a single pattern.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="handleCapitalization">
    /// The same as the second argument from <see cref="FileGlobPattern(string[], bool)"/>.
    /// </param>
    public FileGlobPattern(string pattern, bool handleCapitalization = false)
        : this([pattern], handleCapitalization)
    {
    }

    /// <summary>
    /// Creates the glob-like pattern from smaller patterns that only apply on a specific file type (e.g. *.ico,
    /// *.png). This is useful when you want to specify the same pattern for multiple file types (e.g. Image files
    /// as [*.jpg, *.png etc.]).
    /// </summary>
    /// <param name="patterns">
    /// The individual patterns. The given array will be used directly as a reference if <c>handleCapitalization</c>
    /// is false, so any mutation to the array will also affect this object. Otherwise, the original array is
    /// detached from this object; mutations don't affect this object.
    /// </param>
    /// <param name="handleCapitalization">
    /// If false, will not modify the patterns. If true, will convert each letter of each pattern in "[lL]", where
    /// l is the original letter and L is the capitalized letter (or opposite). Note that this does not take into
    /// account any kind of manual capitalization checks.
    /// </param>
    public FileGlobPattern(string[] patterns, bool handleCapitalization = false)
    {
        if (!handleCapitalization)
        {
            _patterns = patterns;
            return;
        }

        _patterns = new string[patterns.Length];
        var sb = new StringBuilder();

        for (int i = 0; i < patterns.Length; i++)
        {
            sb.Clear();

            foreach (char c in patterns[i])
            {
                if (c == '*' || c == '?' || c == '.' || c == '[' || c == ']' || c == '!' || c == '-')
                {
                    sb.Append(c);
                    continue;
                }

                if (char.IsUpper(c))
                {
                    sb.Append($"[{char.ToLower(c)}{c}]");
                }
                else if (char.IsLower(c))
                {
                    sb.Append($"[{c}{char.ToUpper(c)}]");
                }
                else
                {
                    sb.Append(c);
                }
            }

            _patterns[i] = sb.ToString();
        }
    }

    /// <summary>
    /// Simply returns the patterns as a mutable array. Capitalization is handled only if specified at object creation.
    /// </summary>
    /// <returns>The patterns given at object creation, possibly altered by capitalization if specified.</returns>
    public string[] GetPatternsDirectly()
    {
        return _patterns;
    }

    /// <summary>
    /// Similar to <see cref="ToString(char)"/>, but uses ';' as the separator.
    /// </summary>
    /// <returns>The pattern to be used for filtering files (glob-like pattern).</returns>
    public override string ToString()
    {
        return ToString(';');
    }

    /// <summary>
    /// Returns the pattern to be used for filtering files. It combines all the given patterns and uses the given
    /// separator for them (only if there are more than one, otherwise it will simply return the only pattern).
    /// </summary>
    /// <returns>The pattern to be used for filtering files (glob-like pattern).</returns>
    public string ToString(char separator)
    {
        switch (_patterns.Length)
        {
            case 0:
                return "";
            case 1:
                return _patterns[0];
        }

        var sb = new StringBuilder();
        foreach (string pattern in _patterns)
        {
            sb.Append(pattern);
            sb.Append(separator);
        }

        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}