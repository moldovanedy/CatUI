using CatUI.Data;

namespace CatUI.Elements
{
    public static class El
    {
        public static T Bind<T>(ObservableProperty<T> property) where T : notnull
        {
            //this is only useful when having the code generator
            return property.Value!;
        }
    }
}
