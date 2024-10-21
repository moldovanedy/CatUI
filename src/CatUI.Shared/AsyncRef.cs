namespace CatUI.Shared
{
    /// <summary>
    /// Utility class for passing parameters as reference inside async methods.
    /// It is very similar to the ref keyword, which can't be used on async methods.
    /// The actual value is in <see cref="AsyncRef.Ref"/>
    /// </summary>
    /// <typeparam name="T">
    /// The value that is used as a reference. Changing this from inside a method
    /// will reflect the changes in the calling code.
    /// </typeparam>
    public class AsyncRef<T>
    {
        public T? Ref { get; set; }

        public AsyncRef() { }
        public AsyncRef(T reference)
        {
            Ref = reference;
        }
    }
}