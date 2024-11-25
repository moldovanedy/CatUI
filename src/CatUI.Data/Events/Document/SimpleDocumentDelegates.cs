namespace CatUI.Data.Events.Document
{
    //it doesn't really make sense to have a file for each of these, so they are put here

    public delegate void EnterDocumentEventHandler(object sender);
    public delegate void ExitDocumentEventHandler(object sender);
    public delegate void LoadEventHandler(object sender);
}