using System.IO;

namespace CatUI.Data.Assets
{
    public abstract class Asset
    {
        public abstract void LoadFromRawData(Stream stream);
    }
}
