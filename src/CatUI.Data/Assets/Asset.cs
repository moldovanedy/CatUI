using System.IO;

namespace CatUI.Data.Assets
{
    public abstract class Asset : CatObject
    {
        public abstract void LoadFromRawData(Stream stream);
        public abstract void LoadFromRawData(byte[] rawData);
    }
}
