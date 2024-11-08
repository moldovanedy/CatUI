namespace CatUI.Data.Events.Input
{
    public abstract class InputEventArgs
    {
        public object? Target { get; protected set; }
    }
}