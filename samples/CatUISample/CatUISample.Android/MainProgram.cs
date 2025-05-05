using Android.Content.PM;
using CatUI.Data;
using CatUI.Windowing.Android;
using CatUISample.UI;

namespace CatUISample.Android
{
    [Activity(
        ResizeableActivity = true,
        //tell Android that CatUI is smart enough to handle all configuration changes and that it doesn't need to
        //recreate the activity each time :)
        ConfigurationChanges =
            ConfigChanges.Keyboard |
            ConfigChanges.KeyboardHidden |
            ConfigChanges.Orientation |
            ConfigChanges.ScreenLayout |
            ConfigChanges.ScreenSize |
            ConfigChanges.SmallestScreenSize |
            ConfigChanges.ColorMode |
            ConfigChanges.Density |
            ConfigChanges.FontScale |
            ConfigChanges.FontWeightAdjustment |
            ConfigChanges.GrammaticalGender |
            ConfigChanges.LayoutDirection |
            ConfigChanges.Locale |
            ConfigChanges.Mcc |
            ConfigChanges.Mnc |
            ConfigChanges.Navigation |
            ConfigChanges.UiMode |
            ConfigChanges.Touchscreen,
        Label = "@string/app_name",
        MainLauncher = true)]
    public class MainProgram : AndroidWindow
    {
        public MainProgram()
        {
            //DO NOT use any CatUI APIs until Initialize is called and CatApplication is configured correctly
            Document.OnAppStart += Initialize;
        }

        private void Initialize()
        {
            //early initialization of the app
            CatApplication
                .NewBuilder()
                .SetInitializer(new AndroidPlatformInfo().AppInitializer)
                .Build();

            InitialSetup.Init();
            Document.Root = new RootElement();
        }
    }
}
