namespace CatUI.Data.Events.Document
{
    //it doesn't really make sense to have a file for each of these, so they are put here

    public delegate void DrawEventHandler(object sender);

    public delegate void EnterDocumentEventHandler(object sender);

    public delegate void ExitDocumentEventHandler(object sender);

    public delegate void LoadEventHandler(object sender);

    public delegate void ChildLayoutChangedEventHandler(object sender, ChildLayoutChangedEventArgs e);
}
