using CatUI.Data;
using CatUI.Data.Theming;
using CatUI.Data.Theming.Colors;

namespace CatUISample.UI
{
    public static class InitialSetup
    {
        public static void Init()
        {
            //ReSharper disable All
            CatThemeBuilder.SetColors(
                primary: new ThemeColor(
                    lightStandardContrastColor: new Color(0x3D_5F_90),
                    darkStandardContrastColor: new Color(0xA6_C8_FF),
                    lightMediumContrastColor: new Color(0x0C_37_65),
                    darkMediumContrastColor: new Color(0xCB_DD_FF),
                    lightHighContrastColor: new Color(0x00_2C_58),
                    darkHighContrastColor: new Color(0xEA_F0_FF)),
                onPrimary: new ThemeColor(
                    lightStandardContrastColor: new Color(0xFF_FF_FF),
                    darkStandardContrastColor: new Color(0x02_31_5E),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x00_26_4C),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_00_00)),
                primaryContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0xD5_E3_FF),
                    darkStandardContrastColor: new Color(0x23_47_76),
                    lightMediumContrastColor: new Color(0x4C_6E_A0),
                    darkMediumContrastColor: new Color(0x70_92_C6),
                    lightHighContrastColor: new Color(0x26_4A_79),
                    darkHighContrastColor: new Color(0xA2_C4_FB)),
                onPrimaryContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0x23_47_76),
                    darkStandardContrastColor: new Color(0xD5_E3_FF),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x00_00_00),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_0B_1E)),
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
                secondaryContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0xD4_E3_FF),
                    darkStandardContrastColor: new Color(0x20_48_76),
                    lightMediumContrastColor: new Color(0x4A_6F_9F),
                    darkMediumContrastColor: new Color(0x6F_93_C5),
                    lightHighContrastColor: new Color(0x23_4A_79),
                    darkHighContrastColor: new Color(0xA1_C5_FA)),
                onSecondaryContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0x20_48_76),
                    darkStandardContrastColor: new Color(0xD4_E3_FF),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x00_00_00),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_0B_1D)),
                //
                tertiary: new ThemeColor(
                    lightStandardContrastColor: new Color(0x73_51_87),
                    darkStandardContrastColor: new Color(0xE1_B8_F5),
                    lightMediumContrastColor: new Color(0x48_28_5C),
                    darkMediumContrastColor: new Color(0xF1_D1_FF),
                    lightHighContrastColor: new Color(0x3D_1E_51),
                    darkHighContrastColor: new Color(0xFC_EB_FF)),
                onTertiary: new ThemeColor(
                    lightStandardContrastColor: new Color(0xFF_FF_FF),
                    darkStandardContrastColor: new Color(0x42_22_55),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x36_17_4A),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_00_00)),
                tertiaryContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0xF4_D9_FF),
                    darkStandardContrastColor: new Color(0x5A_39_6E),
                    lightMediumContrastColor: new Color(0x83_5F_97),
                    darkMediumContrastColor: new Color(0xA8_83_BD),
                    lightHighContrastColor: new Color(0x5D_3C_70),
                    darkHighContrastColor: new Color(0xDC_B4_F1)),
                onTertiaryContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0x5A_39_6E),
                    darkStandardContrastColor: new Color(0xF4_D9_FF),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x00_00_00),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x17_00_28)),
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
                errorContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0xFF_DA_D6),
                    darkStandardContrastColor: new Color(0x73_33_2D),
                    lightMediumContrastColor: new Color(0xA2_58_51),
                    darkMediumContrastColor: new Color(0xCC_7B_72),
                    lightHighContrastColor: new Color(0x76_36_2F),
                    darkHighContrastColor: new Color(0xFF_AE_A5)),
                onErrorContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0x73_33_2D),
                    darkStandardContrastColor: new Color(0xFF_DA_D6),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x00_00_00),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x22_00_01)),
                //
                surface: new ThemeColor(
                    lightStandardContrastColor: new Color(0xF9_F9_FF),
                    darkStandardContrastColor: new Color(0x11_13_18),
                    lightMediumContrastColor: new Color(0xF9_F9_FF),
                    darkMediumContrastColor: new Color(0x11_13_18),
                    lightHighContrastColor: new Color(0xF9_F9_FF),
                    darkHighContrastColor: new Color(0x11_13_18)),
                surfaceDim: new ThemeColor(
                    lightStandardContrastColor: new Color(0xD9_D9_E0),
                    darkStandardContrastColor: new Color(0x11_13_18),
                    lightMediumContrastColor: new Color(0xC5_C6_CD),
                    darkMediumContrastColor: new Color(0x11_13_18),
                    lightHighContrastColor: new Color(0xB7_B8_BF),
                    darkHighContrastColor: new Color(0x11_13_18)),
                surfaceBright: new ThemeColor(
                    lightStandardContrastColor: new Color(0xF9_F9_FF),
                    darkStandardContrastColor: new Color(0x37_39_3E),
                    lightMediumContrastColor: new Color(0xF9_F9_FF),
                    darkMediumContrastColor: new Color(0x42_44_4A),
                    lightHighContrastColor: new Color(0xF9_F9_FF),
                    darkHighContrastColor: new Color(0x4E_50_55)),
                surfaceContainerLowest: new ThemeColor(
                    lightStandardContrastColor: new Color(0xFF_FF_FF),
                    darkStandardContrastColor: new Color(0x0C_0E_13),
                    lightMediumContrastColor: new Color(0xFF_FF_FF),
                    darkMediumContrastColor: new Color(0x06_07_0C),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_00_00)),
                surfaceContainerLow: new ThemeColor(
                    lightStandardContrastColor: new Color(0xF3_F3_FA),
                    darkStandardContrastColor: new Color(0x19_1C_20),
                    lightMediumContrastColor: new Color(0xF3_F3_FA),
                    darkMediumContrastColor: new Color(0x1B_1E_22),
                    lightHighContrastColor: new Color(0xF0_F0_F7),
                    darkHighContrastColor: new Color(0x1D_20_24)),
                surfaceContainer: new ThemeColor(
                    lightStandardContrastColor: new Color(0xED_ED_F4),
                    darkStandardContrastColor: new Color(0x1D_20_24),
                    lightMediumContrastColor: new Color(0xE7_E8_EE),
                    darkMediumContrastColor: new Color(0x26_28_2D),
                    lightHighContrastColor: new Color(0xE1_E2_E9),
                    darkHighContrastColor: new Color(0x2E_30_35)),
                surfaceContainerHigh: new ThemeColor(
                    lightStandardContrastColor: new Color(0xE7_E8_EE),
                    darkStandardContrastColor: new Color(0x28_2A_2F),
                    lightMediumContrastColor: new Color(0xDC_DC_E3),
                    darkMediumContrastColor: new Color(0x30_33_38),
                    lightHighContrastColor: new Color(0xD3_D4_DB),
                    darkHighContrastColor: new Color(0x39_3B_41)),
                surfaceContainerHighest: new ThemeColor(
                    lightStandardContrastColor: new Color(0xE1_E2_E9),
                    darkStandardContrastColor: new Color(0x33_35_3A),
                    lightMediumContrastColor: new Color(0xD0_D1_D8),
                    darkMediumContrastColor: new Color(0x3C_3E_43),
                    lightHighContrastColor: new Color(0xC5_C6_CD),
                    darkHighContrastColor: new Color(0x45_47_4C)),
                //
                onSurface: new ThemeColor(
                    lightStandardContrastColor: new Color(0x19_1C_20),
                    darkStandardContrastColor: new Color(0xE1_E2_E9),
                    lightMediumContrastColor: new Color(0x0F_11_16),
                    darkMediumContrastColor: new Color(0xFF_FF_FF),
                    lightHighContrastColor: new Color(0x00_00_00),
                    darkHighContrastColor: new Color(0xFF_FF_FF)),
                onSurfaceVariant: new ThemeColor(
                    lightStandardContrastColor: new Color(0x43_47_4E),
                    darkStandardContrastColor: new Color(0xC3_C6_CF),
                    lightMediumContrastColor: new Color(0x32_36_3D),
                    darkMediumContrastColor: new Color(0xD9_DC_E5),
                    lightHighContrastColor: new Color(0x00_00_00),
                    darkHighContrastColor: new Color(0xFF_FF_FF)),
                //
                inverseSurface: new ThemeColor(
                    lightStandardContrastColor: new Color(0x2E_30_35),
                    darkStandardContrastColor: new Color(0xE1_E2_E9),
                    lightMediumContrastColor: new Color(0x2E_30_35),
                    darkMediumContrastColor: new Color(0xE1_E2_E9),
                    lightHighContrastColor: new Color(0x2E_30_35),
                    darkHighContrastColor: new Color(0xE1_E2_E9)),
                inverseOnSurface: new ThemeColor(
                    lightStandardContrastColor: new Color(0xF0_F0_F7),
                    darkStandardContrastColor: new Color(0x2E_30_35),
                    lightMediumContrastColor: new Color(0xF0_F0_F7),
                    darkMediumContrastColor: new Color(0x28_2A_2F),
                    lightHighContrastColor: new Color(0xFF_FF_FF),
                    darkHighContrastColor: new Color(0x00_00_00)),
                inversePrimary: new ThemeColor(
                    lightStandardContrastColor: new Color(0xA6_C8_FF),
                    darkStandardContrastColor: new Color(0x3D_5F_90),
                    lightMediumContrastColor: new Color(0xA6_C8_FF),
                    darkMediumContrastColor: new Color(0x24_49_78),
                    lightHighContrastColor: new Color(0xA6_C8_FF),
                    darkHighContrastColor: new Color(0x24_49_78)),
                //
                outline: new ThemeColor(
                    lightStandardContrastColor: new Color(0x73_77_7F),
                    darkStandardContrastColor: new Color(0x8D_91_99),
                    lightMediumContrastColor: new Color(0x4E_53_5A),
                    darkMediumContrastColor: new Color(0xAE_B2_BA),
                    lightHighContrastColor: new Color(0x28_2C_33),
                    darkHighContrastColor: new Color(0xED_F0_F9)),
                outlineVariant: new ThemeColor(
                    lightStandardContrastColor: new Color(0xC3_C6_CF),
                    darkStandardContrastColor: new Color(0x43_47_4E),
                    lightMediumContrastColor: new Color(0x69_6D_75),
                    darkMediumContrastColor: new Color(0x8C_90_98),
                    lightHighContrastColor: new Color(0x45_49_51),
                    darkHighContrastColor: new Color(0xBF_C3_CB)),
                scrim: new ThemeColor(
                    lightStandardContrastColor: new Color(0x00_00_00),
                    darkStandardContrastColor: new Color(0x00_00_00),
                    lightMediumContrastColor: new Color(0x00_00_00),
                    darkMediumContrastColor: new Color(0x00_00_00),
                    lightHighContrastColor: new Color(0x00_00_00),
                    darkHighContrastColor: new Color(0x00_00_00)),
                shadow: new ThemeColor(
                    lightStandardContrastColor: new Color(0x00_00_00),
                    darkStandardContrastColor: new Color(0x00_00_00),
                    lightMediumContrastColor: new Color(0x00_00_00),
                    darkMediumContrastColor: new Color(0x00_00_00),
                    lightHighContrastColor: new Color(0x00_00_00),
                    darkHighContrastColor: new Color(0x00_00_00))
            );
            //ReSharper enable All

            CatThemeBuilder.ApplyTheme();
            CatTheme.Settings.IsDarkModeEnabled = new CatPlatformDependentSetting<bool>(true, true);
        }
    }
}
