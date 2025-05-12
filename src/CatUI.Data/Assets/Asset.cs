using System.IO;
using System.Threading.Tasks;

namespace CatUI.Data.Assets
{
    public abstract class Asset : CatObject
    {
        public bool IsLoaded { get; protected set; }

        protected internal abstract void LoadFromStream(Stream stream);
        protected internal abstract Task LoadFromStreamAsync(Stream stream);
        protected internal abstract void LoadFromRawData(byte[] rawData);
    }
}
