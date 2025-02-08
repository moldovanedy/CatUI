namespace CatUI.Utils
{
    /// <summary>
    /// An object that holds a reference to another object of type T. It is similar to the ref keyword.
    /// It can be used even in async code to pass parameters. The actual value is in <see cref="Value"/>.
    /// </summary>
    /// <remarks>
    /// It is very useful for CatUI elements as you can declare this value in a more elevated scope
    /// (like a field of the class or even something publicly accessible), set it from a specific point (like an element's
    /// constructor or EnterDocument) and get it in different contexts (like in other methods).
    /// </remarks>
    /// <typeparam name="T">
    /// The value that is used as a reference. Changing this from inside a method will reflect the changes in the calling code.
    /// </typeparam>
    public class ObjectRef<T>
    {
        public T? Value { get; set; }

        public ObjectRef() { }

        public ObjectRef(T reference)
        {
            Value = reference;
        }
    }
}
