using System.IO;
using System.Threading.Tasks;

namespace CatUI.Data.Assets;

public class I18NStreamAsset : Asset
{
    public MemoryStream? MemStream { get; private set; }

    /// <summary>
    /// Creates a new, empty internationalization stream asset, without any data.
    /// </summary>
    public I18NStreamAsset()
    {
        IsLoaded = true;
    }

    /// <summary>
    /// Creates a new internationalization stream asset with the given stream held as a reference.
    /// </summary>
    public I18NStreamAsset(MemoryStream stream)
    {
        IsLoaded = true;
        MemStream = stream;
    }

    public override CatObject Duplicate()
    {
        return MemStream == null ? new I18NStreamAsset() : new I18NStreamAsset(MemStream);
    }

    protected internal override void LoadFromStream(Stream stream)
    {
        MemStream = new MemoryStream();
        stream.CopyTo(MemStream);
        MemStream.Seek(0, SeekOrigin.Begin);
    }

    protected internal override async Task LoadFromStreamAsync(Stream stream)
    {
        MemStream = new MemoryStream();
        await stream.CopyToAsync(MemStream);
        MemStream.Seek(0, SeekOrigin.Begin);
    }

    protected internal override void LoadFromRawData(byte[] rawData)
    {
        MemStream = new MemoryStream();
        MemStream.Write(rawData);
        MemStream.Seek(0, SeekOrigin.Begin);
    }
}
