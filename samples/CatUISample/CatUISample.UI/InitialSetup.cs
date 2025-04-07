using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Data.Theming.Colors;
using CatUI.Windowing.Desktop;

namespace CatUISample.UI
{
    public static class InitialSetup
    {
        public static void Init()
        {
            //early initialization of the app
            CatApplication
                .NewBuilder()
                .SetInitializer(new DesktopPlatformInfo().AppInitializer)
                .Build();

            //ReSharper disable All
            CatThemeBuilder.SetColors(
                primary: new ThemeColor(
                    lightStandardContrastColor: new Color(0x3D_5F_90),
                    darkStandardContrastColor: new Color(0xA6_C8_FF),
                    lightMediumContrastColor: new Color(0x0C_37_65),
                    darkMediumContrastColor: new Color(0xCB_DD_FF),
                    lightHighContrastColor: new Color(0x00_2C_58),
                    darkHighContrastColor: new Color(0xEA_E0_FF)),
                onPrimary: new ThemeColor(
                    lightStandardContrastColor: new Color(0xFF_FF_FF),
                    darkStandardContrastColor: new Color(0x02_31_5E),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x00_26_4C),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_00_00)),
                //
                secondary: new ThemeColor(
                    lightStandardContrastColor: new Color(0x3B_60_8F),
                    darkStandardContrastColor: new Color(0xA5_C9_FE),
                    lightMediumContrastColor: new Color(0x08_37_64),
                    darkMediumContrastColor: new Color(0xCA_DD_FF),
                    lightHighContrastColor: new Color(0x00_2D_56),
                    darkHighContrastColor: new Color(0xEA_F0_FF)),
                onSecondary: new ThemeColor(
                    lightStandardContrastColor: new Color(0xFF_FF_FF),
                    darkStandardContrastColor: new Color(0x00_31_5D),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x00_26_4B),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_00_00)),
                //
                error: new ThemeColor(
                    lightStandardContrastColor: new Color(0x90_4A_43),
                    darkStandardContrastColor: new Color(0xFF_B4_AB),
                    lightMediumContrastColor: new Color(0x5E_23_1E),
                    darkMediumContrastColor: new Color(0xFF_D2_CC),
                    lightHighContrastColor: new Color(0x51_1A_15),
                    darkHighContrastColor: new Color(0xFF_EC_E9)),
                onError: new ThemeColor(
                    lightStandardContrastColor: new Color(0xFF_FF_FF),
                    darkStandardContrastColor: new Color(0x56_1E_19),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x48_13_0F),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_00_00)),
                //
                surface: new ThemeColor(
                    lightStandardContrastColor: new Color(0xF9_F9_FF),
                    darkStandardContrastColor: new Color(0x11_13_18),
                    lightMediumContrastColor: new Color(0xF9_F9_FF),
                    darkMediumContrastColor: new Color(0x11_13_18),
                    lightHighContrastColor: new Color(0xF9_F9_FF),
                    darkHighContrastColor: new Color(0x11_13_18)),
                onSurface: new ThemeColor(
                    lightStandardContrastColor: new Color(0x19_1C_20),
                    darkStandardContrastColor: new Color(0xE1_E2_E9),
                    lightMediumContrastColor: new Color(0x0F_11_16),
                    darkMediumContrastColor: new Color(0xFF_FF_FF),
                    lightHighContrastColor: new Color(0x00_00_00),
                    darkHighContrastColor: new Color(0xFF_FF_FF)),
                //
                surfaceContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0xED_ED_F4),
                    darkStandardContrastColor: new Color(0x1D_20_24),
                    lightMediumContrastColor: new Color(0xE7_E8_EE),
                    darkMediumContrastColor: new Color(0x26_28_2D),
                    lightHighContrastColor: new Color(0xE1_E2_E9),
                    darkHighContrastColor: new Color(0x2E_30_35)),
                outline: new ThemeColor(
                    lightStandardContrastColor: new Color(0x73_77_7F),
                    darkStandardContrastColor: new Color(0x8D_91_99),
                    lightMediumContrastColor: new Color(0x4E_53_5A),
                    darkMediumContrastColor: new Color(0xAE_B2_BA),
                    lightHighContrastColor: new Color(0x28_2C_33),
                    darkHighContrastColor: new Color(0xED_F0_F9))
            );

            CatThemeBuilder.ApplyTheme();
            CatTheme.Settings.IsDarkModeEnabled = PlatformOption.PlatformDependentFallbackEnabled;
            //ReSharper enable All
        }
    }
}
