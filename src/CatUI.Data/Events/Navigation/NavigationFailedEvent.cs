namespace CatUI.Data.Events.Navigation
{
    public delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);

    public class NavigationFailedEventArgs : AbstractNavigationEventArgs
    {
        /// <summary>
        /// Contains the path on which the navigator tried to navigate to, but failed.
        /// </summary>
        public string FailedPath { get; }

        public NavigationFailedEventArgs(string oldPath, string failedPath)
        {
            OldPath = oldPath;
            FailedPath = failedPath;
        }
    }
}
