using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CatUIUtility
{
    public static class ThemeCommand
    {
        private static readonly List<string> _tokens =
        [
            "primary", "onPrimary", "primaryContainer", "onPrimaryContainer",
            "secondary", "onSecondary", "secondaryContainer", "onSecondaryContainer",
            "tertiary", "onTertiary", "tertiaryContainer", "onTertiaryContainer",
            "error", "onError", "errorContainer", "onErrorContainer",
            //success is not in Material 3
            "surface", "surfaceDim", "surfaceBright",
            "surfaceContainerLowest", "surfaceContainerLow", "surfaceContainer", "surfaceContainerHigh",
            "surfaceContainerHighest",
            "onSurface", "onSurfaceVariant",
            "inverseSurface", "inverseOnSurface", "inversePrimary",
            "outline", "outlineVariant", "scrim", "shadow"
        ];

        public static void Start(string[] args)
        {
            //show command help
            if (args.Length == 1 || args[1] == "-h" || args[1] == "--help")
            {
                Console.WriteLine(
                    "Usage: catui-utility theme [arguments]\n" +
                    "\n" +
                    "[arguments] are one or more of the following:\n" +
                    //
                    "- --no-named-arguments: If specified (must be the first argument), the code won't have named arguments " +
                    "and instead base on positional arguments. Named arguments generally help visualize to what tokens " +
                    "the colors are applied, so it's generally a good ideea to have them, but you can disable them with " +
                    "this option.\n" +
                    //
                    "- --material-theme-descriptor-path: The relative path to the JSON file from the Material Theme Builder " +
                    "(https://material-foundation.github.io/material-theme-builder/). If the path contains spaces or " +
                    "other special characters, write it inside quotes (\"\").\n" +
                    //
                    "- --use-material-3-typography: If specified, the typography options will be the same as Google's " +
                    "Material Design 3. If this is not used, the code for setting typography options will not be shown, " +
                    "leaving the default CatTheme values. Note that CatTheme doesn't have a 1:1 mapping to Material 3, " +
                    "so some values will be missing.\n" +
                    //
                    "- --use-material-3-shapes: If specified, the shapes' corner radii will be the same as Google's " +
                    "Material Design 3. If this is not used, the code for setting shapes will not be shown, leaving " +
                    "the default CatTheme values. Note that CatTheme doesn't have a 1:1 mapping to Material 3, so some " +
                    "values will be missing.\n" +
                    //
                    "If something will not work properly or some arguments are invalid, a message starting with " +
                    "\"Error\" will be shown instead of the code.\n"
                );
                return;
            }

            StringBuilder code = new("Code:\n-----\n");
            bool noNamedArguments = args[1] == "--no-named-arguments";
            bool usedColors = false, usedTypography = false;

            int i = noNamedArguments ? 2 : 1;
            while (i < args.Length)
            {
                switch (args[i])
                {
                    case "--material-theme-descriptor-path":
                        if (args.Length == i)
                        {
                            Console.WriteLine("Error: Path to Material 3 JSON descriptor file is missing.");
                            return;
                        }

                        string? colorsCode = GetMaterialColorsCode(args[i + 1], noNamedArguments);
                        if (colorsCode == null)
                        {
                            return;
                        }

                        code.Append(colorsCode);
                        usedColors = true;
                        i++;
                        break;
                    case "--use-material-3-typography":
                        code.Append(GetMaterialTypographyCode(noNamedArguments));
                        usedTypography = true;
                        break;
                    case "--use-material-3-shapes":
                        code.Append(GetMaterialShapesCode(noNamedArguments));
                        break;
                    default:
                        Console.WriteLine(
                            $"Error: Unknown argument \"{args[i]}\". Valid arguments are: " +
                            $"\"--material-theme-descriptor-path\", \"--use-material-3-shapes\" and " +
                            $"\"--use-material-3-typography\".\n");
                        return;
                }

                i++;
            }

            code.Append("CatTheme.ApplyTheme()\n");
            //end the code section
            code.Append("-----\n");
            Console.WriteLine(code);

            string colorUsing = usedColors ? "  using CatUI.Data.Theming.Colors;\n" : "";
            string typographyUsing = usedTypography ? "  using CatUI.Data.Theming.Typography;\n" : "";

            Console.WriteLine(
                "You must also specify the following using directives:\n" +
                "  using CatUI.Data;\n" +
                "  using CatUI.Data.Theming;\n" +
                colorUsing +
                typographyUsing +
                "\nHappy coding!");
        }

        private static string? GetMaterialColorsCode(string jsonFilePath, bool noNamedArguments = false)
        {
            if (jsonFilePath.StartsWith("./"))
            {
                jsonFilePath = Path.Combine(Environment.CurrentDirectory, jsonFilePath.Substring(2));
            }

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine("Error: Material 3 JSON file does not exist.");
                return null;
            }

            try
            {
                string contents = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                JObject obj = JObject.Parse(contents);
                var combinations = obj["schemes"]?.ToObject<JObject>();
                if (combinations == null)
                {
                    Console.WriteLine("Error: Material 3 JSON file is missing the \"schemes\" object.");
                    return null;
                }

                StringBuilder builder = new(
                    noNamedArguments
                        ? "CatThemeBuilder.SetColors(\n"
                        : "//this forces ReSharper refactoring to NOT remove named arguments regardless of local " +
                          "project configuration\n" +
                          "//ReSharper disable All\n" +
                          "CatThemeBuilder.SetColors(\n");

                foreach (string token in _tokens)
                {
                    StringBuilder? codeForColor = GetCodeForColor(token, combinations, noNamedArguments);
                    if (codeForColor != null)
                    {
                        if (codeForColor.Length == 0)
                        {
                            if (noNamedArguments)
                            {
                                builder.Append("    null,\n");
                            }

                            Console.WriteLine(
                                $"INFO: Color with token \"{token}\" was not found. Code is still valid.");
                        }
                        else
                        {
                            builder.Append(codeForColor);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Partial code:\n{builder}");
                        return null;
                    }
                }

                //remove the trailing ","
                builder.Remove(builder.Length - 2, 1);
                //add the end
                builder.Append(");\n");

                if (!noNamedArguments)
                {
                    builder.Append("//ReSharper enable All\n");
                }

                builder.Append('\n');
                return builder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Exception while parsing Material 3 JSON file: {ex}");
                return null;
            }
        }

        private static StringBuilder? GetCodeForColor(string colorToken, JObject data, bool noNamedArguments = false)
        {
            //if there are no occurrences of colorToken, it means this color token is absent, represented by an empty SB
            if (data["light"]?[colorToken] == null && data["dark"]?[colorToken] == null)
            {
                return new StringBuilder();
            }

            StringBuilder code = new(
                noNamedArguments
                    ? "    new ThemeColor(\n"
                    : $"    {colorToken}: new ThemeColor(\n");

            //light standard contrast
            string? hexString = data["light"]?[colorToken]?.Value<string>();
            if (hexString == null)
            {
                Console.WriteLine($"Error: Parse error when trying to get [\"schema\"][\"light\"][\"{colorToken}\"].");
                return null;
            }

            if (!hexString.StartsWith('#') || hexString.Length != 7)
            {
                Console.WriteLine(
                    $"Error: Invalid hex string \"{hexString}\" at [\"schema\"][\"light\"][\"{colorToken}\"].");
                return null;
            }

            string codeSnippet =
                $"new Color(0x{hexString.AsSpan(1, 2)}_{hexString.AsSpan(3, 2)}_{hexString.AsSpan(5, 2)}),\n";

            code.Append("        ");
            if (noNamedArguments)
            {
                code.Append(codeSnippet);
            }
            else
            {
                code.Append("lightStandardContrastColor: " + codeSnippet);
            }

            //dark standard contrast
            hexString = data["dark"]?[colorToken]?.Value<string>();
            if (hexString == null)
            {
                Console.WriteLine($"Error: Parse error when trying to get [\"schema\"][\"dark\"][\"{colorToken}\"].");
                return null;
            }

            if (!hexString.StartsWith('#') || hexString.Length != 7)
            {
                Console.WriteLine(
                    $"Error: Invalid hex string \"{hexString}\" at [\"schema\"][\"dark\"][\"{colorToken}\"].");
                return null;
            }

            codeSnippet = $"new Color(0x{hexString.AsSpan(1, 2)}_{hexString.AsSpan(3, 2)}_{hexString.AsSpan(5, 2)}),\n";

            code.Append("        ");
            if (noNamedArguments)
            {
                code.Append(codeSnippet);
            }
            else
            {
                code.Append("darkStandardContrastColor: " + codeSnippet);
            }

            bool hasOptionalLightColor = true;
            //light medium contrast
            hexString = data["light-medium-contrast"]?[colorToken]?.Value<string>();
            if (hexString == null)
            {
                hasOptionalLightColor = false;
                Console.WriteLine("INFO: No light with medium contrast color found. Code is still valid.");
            }
            else
            {
                if (!hexString.StartsWith('#') || hexString.Length != 7)
                {
                    Console.WriteLine(
                        $"Error: Invalid hex string \"{hexString}\" at [\"schema\"][\"light-medium-contrast\"][\"{colorToken}\"].");
                    return null;
                }

                codeSnippet =
                    $"new Color(0x{hexString.AsSpan(1, 2)}_{hexString.AsSpan(3, 2)}_{hexString.AsSpan(5, 2)}),\n";

                code.Append("        ");
                if (noNamedArguments)
                {
                    code.Append(codeSnippet);
                }
                else
                {
                    code.Append("lightMediumContrastColor: " + codeSnippet);
                }
            }

            //dark medium contrast
            hexString = data["dark-medium-contrast"]?[colorToken]?.Value<string>();
            if (hexString == null)
            {
                if (hasOptionalLightColor)
                {
                    Console.WriteLine(
                        "Error: You have light with medium contrast color, but no dark with medium contrast color. " +
                        "Or it might be a parse error at dark medium contrast.");
                    return null;
                }

                Console.WriteLine("INFO: No dark with medium contrast color found. Code is still valid.");
            }
            else
            {
                if (!hasOptionalLightColor)
                {
                    Console.WriteLine(
                        "Error: You have dark with medium contrast color, but no light with medium contrast color. " +
                        "Or it might be a parse error at light medium contrast.");
                    return null;
                }

                if (!hexString.StartsWith('#') || hexString.Length != 7)
                {
                    Console.WriteLine(
                        $"Error: Invalid hex string \"{hexString}\" at [\"schema\"][\"dark-medium-contrast\"][\"{colorToken}\"].");
                    return null;
                }

                codeSnippet =
                    $"new Color(0x{hexString.AsSpan(1, 2)}_{hexString.AsSpan(3, 2)}_{hexString.AsSpan(5, 2)}),\n";

                code.Append("        ");
                if (noNamedArguments)
                {
                    code.Append(codeSnippet);
                }
                else
                {
                    code.Append("darkMediumContrastColor: " + codeSnippet);
                }
            }

            //light high contrast
            hexString = data["light-high-contrast"]?[colorToken]?.Value<string>();
            if (hexString == null)
            {
                hasOptionalLightColor = false;
                Console.WriteLine("INFO: No light with high contrast color found. Code is still valid.");
            }
            else
            {
                if (!hexString.StartsWith('#') || hexString.Length != 7)
                {
                    Console.WriteLine(
                        $"Error: Invalid hex string \"{hexString}\" at [\"schema\"][\"light-high-contrast\"][\"{colorToken}\"].");
                    return null;
                }

                codeSnippet =
                    $"new Color(0x{hexString.AsSpan(1, 2)}_{hexString.AsSpan(3, 2)}_{hexString.AsSpan(5, 2)}),\n";

                code.Append("        ");
                if (noNamedArguments)
                {
                    code.Append(codeSnippet);
                }
                else
                {
                    code.Append("lightHighContrastColor: " + codeSnippet);
                }
            }

            //dark high contrast
            hexString = data["dark-high-contrast"]?[colorToken]?.Value<string>();
            if (hexString == null)
            {
                if (hasOptionalLightColor)
                {
                    Console.WriteLine(
                        "Error: You have light with high contrast color, but no dark with high contrast color. " +
                        "Or it might be a parse error at dark high contrast.");
                    return null;
                }

                Console.WriteLine("INFO: No dark with high contrast color found. Code is still valid.");
            }
            else
            {
                if (!hasOptionalLightColor)
                {
                    Console.WriteLine(
                        "Error: You have dark with high contrast color, but no light with high contrast color. " +
                        "Or it might be a parse error at light high contrast.");
                    return null;
                }

                if (!hexString.StartsWith('#') || hexString.Length != 7)
                {
                    Console.WriteLine(
                        $"Error: Invalid hex string \"{hexString}\" at [\"schema\"][\"dark-high-contrast\"][\"{colorToken}\"].");
                    return null;
                }

                code.Append("        ");
                codeSnippet =
                    $"new Color(0x{hexString.AsSpan(1, 2)}_{hexString.AsSpan(3, 2)}_{hexString.AsSpan(5, 2)}),\n";

                if (noNamedArguments)
                {
                    code.Append(codeSnippet);
                }
                else
                {
                    code.Append("darkHighContrastColor: " + codeSnippet);
                }
            }

            //add after ",\n"
            code.Insert(code.Length - 2, ")");
            return code;
        }

        private static string GetMaterialTypographyCode(bool noNamedArguments = false)
        {
            if (noNamedArguments)
            {
                return
                    "CatThemeBuilder.SetTypographyRules(\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 57f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 45f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 36f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 32f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 28f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 24f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 16f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 14f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.NORMAL, 12f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.MEDIUM, 14f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.MEDIUM, 12f, 1.125f),\n" +
                    "    new ThemeTextStyle(FontWeightPresets.MEDIUM, 11f, 1.125f)\n" +
                    ");\n\n";
            }

            return
                "//this forces ReSharper refactoring to NOT remove named arguments regardless of local project configuration\n" +
                "//ReSharper disable All\n" +
                "CatThemeBuilder.SetTypographyRules(\n" +
                "    displayLarge: new ThemeTextStyle(FontWeightPresets.NORMAL, 57f, 1.125f),\n" +
                "    displayMedium: new ThemeTextStyle(FontWeightPresets.NORMAL, 45f, 1.125f),\n" +
                "    displaySmall: new ThemeTextStyle(FontWeightPresets.NORMAL, 36f, 1.125f),\n" +
                "    headingLarge: new ThemeTextStyle(FontWeightPresets.NORMAL, 32f, 1.125f),\n" +
                "    headingMedium: new ThemeTextStyle(FontWeightPresets.NORMAL, 28f, 1.125f),\n" +
                "    headingSmall: new ThemeTextStyle(FontWeightPresets.NORMAL, 24f, 1.125f),\n" +
                "    bodyLarge: new ThemeTextStyle(FontWeightPresets.NORMAL, 16f, 1.125f),\n" +
                "    bodyMedium: new ThemeTextStyle(FontWeightPresets.NORMAL, 14f, 1.125f),\n" +
                "    bodySmall: new ThemeTextStyle(FontWeightPresets.NORMAL, 12f, 1.125f),\n" +
                "    labelLarge: new ThemeTextStyle(FontWeightPresets.MEDIUM, 14f, 1.125f),\n" +
                "    labelMedium: new ThemeTextStyle(FontWeightPresets.MEDIUM, 12f, 1.125f),\n" +
                "    labelSmall: new ThemeTextStyle(FontWeightPresets.MEDIUM, 11f, 1.125f)\n" +
                ");\n" +
                "//ReSharper enable All\n\n";
        }

        private static string GetMaterialShapesCode(bool noNamedArguments = false)
        {
            if (noNamedArguments)
            {
                return "CatThemeBuilder.SetClipShapes(8, 12, 16, 28);\n\n";
            }

            return
                "//this forces ReSharper refactoring to NOT remove named arguments regardless of local project configuration\n" +
                "//ReSharper disable All\n" +
                "CatThemeBuilder.SetClipShapes(\n" +
                "    smallRoundingSize: 8,\n" +
                "    mediumRoundingSize: 12,\n" +
                "    largeRoundingSize: 16,\n" +
                "    xlRoundingSize: 28\n" +
                ");\n" +
                "//ReSharper enable All\n\n";
        }
    }
}
