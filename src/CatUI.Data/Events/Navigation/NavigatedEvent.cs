namespace CatUI.Data.Events.Navigation
{
    public delegate void NavigatedEventHandler(object sender, NavigatedEventArgs e);

    public class NavigatedEventArgs : AbstractNavigationEventArgs
    {
        public string NewPath { get; }

        public NavigatedEventArgs(string oldPath, string newPath)
        {
            OldPath = oldPath;
            NewPath = newPath;
        }
    }
}
