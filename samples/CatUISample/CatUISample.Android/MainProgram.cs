using CatUI.Data;
using CatUI.Windowing.Android;
using CatUISample.UI;

namespace CatUISample.Android
{
    [Activity(ResizeableActivity = true, Label = "@string/app_name", MainLauncher = true)]
    public class MainProgram : AndroidWindow
    {
        //OnCreate will get called multiple times, so we need to make sure we only set the initializer once
        private static bool _isCreatedSingleton;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            if (!_isCreatedSingleton)
            {
                //early initialization of the app
                CatApplication
                    .NewBuilder()
                    .SetInitializer(new AndroidPlatformInfo().AppInitializer)
                    .Build();
                _isCreatedSingleton = true;
            }

            InitialSetup.Init();
            base.OnCreate(savedInstanceState);

            Document.Root = new RootElement();
        }
    }
}
