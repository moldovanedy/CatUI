using System.Collections.Generic;

namespace CatUI.Data.Navigator
{
    /// <summary>
    /// Utility for Navigator to give arguments to routes. Its use is entirely optional, but good use of
    /// arguments can make the navigation process easy to understand, efficient and fast.
    /// </summary>
    public class NavArgs
    {
        public Dictionary<string, object> Arguments { get; } = new();

        public NavArgs() { }

        public NavArgs(params KeyValuePair<string, object>[] args)
        {
            foreach (KeyValuePair<string, object> argument in args)
            {
                Arguments.Add(argument.Key, argument.Value);
            }
        }

        /// <summary>
        /// Utility for setting arguments in <see cref="Arguments"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetArgument(string key, object value)
        {
            Arguments[key] = value;
        }

        /// <summary>
        /// Utility for getting arguments from <see cref="Arguments"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The argument or null if one is not found.</returns>
        public object? GetArgument(string key)
        {
            return Arguments.GetValueOrDefault(key);
        }
    }
}
