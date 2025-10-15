using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.Platform.CommonInterface;
using CatUI.Platform.NativeUI;
using CatUI.Utils;

namespace CatUISample.UI.Pages.NativeUi;

public class FilePickingExamples : ScrollContainer
{
    public FilePickingExamples()
    {
        ElementLayout buttonsLayout = new ElementLayout().SetFixedWidth(200).SetFixedHeight(32);

        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");

        Content = new PaddingElement(new EdgeInset(0, 5))
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
            Children =
            [
                new ColumnContainer
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                    Arrangement = LinearArrangement.SpacedBy(5),
                    Children =
                    [
                        new Label("File picking examples", TextAlignmentType.Center)
                        {
                            Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                            FontSize = 32,
                            TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                        },
                        new Button("Open file", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                        {
                            Layout = buttonsLayout,
                            Background = new ColorBrush(CatTheme.Colors.Primary),
                            OnClick = async void (_, _) =>
                            {
                                try
                                {
                                    NativeOpenFileDialog openFileDialog = new()
                                    {
                                        DialogTitle = "Open files",
                                        CanSelectMultipleItems = true,
                                        CustomSubmitButtonText = "Get file",
                                        FilterPattern = new IFilePicker.FileFiltersArgument(
                                        [
                                            new IFilePicker.FileFilter(
                                                "All files",
                                                new FileGlobPattern("*.*")),
                                            new IFilePicker.FileFilter(
                                                "Image files",
                                                new FileGlobPattern(["*.png", "*.jpg"]))
                                        ], 1)
                                    };

                                    try
                                    {
                                        IFilePicker.OpenFilesResponse? result = await openFileDialog.OpenAsync();
                                        if (result == null)
                                        {
                                            CatLogger.LogDebug("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Files:");
                                        foreach (FilePath filePath in result.FilePaths)
                                        {
                                            CatLogger.LogDebug(filePath.ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogError($"Failed to open file picker. Exception: {ex}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CatLogger.LogException(ex);
                                }
                            }
                        },
                        new Button("Open directory", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                        {
                            Layout = buttonsLayout,
                            Background = new ColorBrush(CatTheme.Colors.Primary),
                            OnClick = async void (_, _) =>
                            {
                                try
                                {
                                    NativeOpenDirectoryDialog openDirectoryDialog = new()
                                    {
                                        DialogTitle = "Open directory", CanSelectMultipleItems = true
                                    };

                                    try
                                    {
                                        IFilePicker.OpenDirectoriesResponse? result =
                                            await openDirectoryDialog.OpenAsync();
                                        if (result == null)
                                        {
                                            CatLogger.LogDebug("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Directories:");
                                        foreach (FilePath filePath in result.DirectoryPaths)
                                        {
                                            CatLogger.LogDebug(filePath.ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogError($"Failed to open file picker. Exception: {ex}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CatLogger.LogException(ex);
                                }
                            }
                        },
                        new Button("Open files with choices", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                        {
                            Layout = buttonsLayout,
                            Background = new ColorBrush(CatTheme.Colors.Primary),
                            OnClick = async void (_, _) =>
                            {
                                try
                                {
                                    NativeOpenFileDialog openFileDialog = new()
                                    {
                                        DialogTitle = "Open files (with choices)",
                                        CanSelectMultipleItems = true,
                                        PickerChoices =
                                        [
                                            new IFilePicker.PickerChoicesRequest(
                                                "opt1",
                                                "Option 1",
                                                [
                                                    ("opt1-val1", "Value 1"),
                                                    ("opt1-val2", "Value 2"),
                                                    ("opt1-val3", "Value 3")
                                                ],
                                                0),
                                            new IFilePicker.PickerChoicesRequest(
                                                "opt2",
                                                "Option 2",
                                                [],
                                                0)
                                        ]
                                    };

                                    try
                                    {
                                        IFilePicker.OpenFilesResponse? result = await openFileDialog.OpenAsync();
                                        if (result == null)
                                        {
                                            CatLogger.LogDebug("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Files:");
                                        foreach (FilePath filePath in result.FilePaths)
                                        {
                                            CatLogger.LogDebug(filePath.ToString());
                                        }

                                        if (result.PickerChoicesResponse == null)
                                        {
                                            CatLogger.LogDebug("This platform does not support choices.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Choices:");
                                        foreach (KeyValuePair<string, string> choice in
                                                 result.PickerChoicesResponse)
                                        {
                                            CatLogger.LogDebug($"{choice.Key}: {choice.Value}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogError($"Failed to open file picker. Exception: {ex}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CatLogger.LogException(ex);
                                }
                            }
                        },
                        new Button("Save file", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                        {
                            Layout = buttonsLayout,
                            Background = new ColorBrush(CatTheme.Colors.Primary),
                            OnClick = async void (_, _) =>
                            {
                                try
                                {
                                    NativeSaveFileDialog saveFileDialog = new()
                                    {
                                        DialogTitle = "Save file", SuggestedFileName = "file1.png"
                                    };

                                    try
                                    {
                                        IFilePicker.SaveFileResponse? result = await saveFileDialog.OpenAsync();
                                        if (result == null)
                                        {
                                            CatLogger.LogDebug("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug($"Success! File: {result.FilePath}");
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogError($"Failed to open file picker. Exception: {ex}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CatLogger.LogException(ex);
                                }
                            }
                        }
                    ]
                }
            ]
        };
    }
}
