using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Containers;
using CatUI.Elements.Shapes;
using CatUI.Elements.Text;

namespace CatUISample.UI.Pages.Layout
{
    public class RowContainersExamples : ColumnContainer
    {
        public RowContainersExamples()
        {
            Layout = new ElementLayout().SetFixedWidth("100%");
            Arrangement = LinearArrangement.SpacedBy(20);
        }

        protected override void EnterDocument(object sender)
        {
            base.EnterDocument(sender);

            Children =
            [
                new TextBlock("RowContainer examples", TextAlignmentType.Center)
                {
                    Layout = new ElementLayout().SetMinMaxWidth(0, "100%", true),
                    FontSize = 32,
                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                },

                GetRowContainerExample(LinearArrangement.JustificationType.Start),
                GetRowContainerExample(LinearArrangement.JustificationType.Center),
                GetRowContainerExample(LinearArrangement.JustificationType.End),

                GetSpacingRowContainerExample(LinearArrangement.JustificationType.SpaceAround),
                GetSpacingRowContainerExample(LinearArrangement.JustificationType.SpaceBetween),
                GetSpacingRowContainerExample(LinearArrangement.JustificationType.SpaceEvenly)
            ];
        }

        private static ColumnContainer GetRowContainerExample(LinearArrangement.JustificationType justification)
        {
            return new ColumnContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%"),
                Arrangement = LinearArrangement.SpacedBy(10),
                Children =
                [
                    new TextBlock($"Alignment: {justification}")
                    {
                        FontSize = 16, TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                    },
                    new RowContainer
                    {
                        //TODO: this is not added to the cache, as it should've thrown DuplicateIdException
                        Id = "Row",
                        Layout = new ElementLayout().SetFixedWidth("100%"),
                        Arrangement = new LinearArrangement(justification, 0),
                        Background = new ColorBrush(CatTheme.Colors.SurfaceContainer),
                        Children =
                        [
                            new RectangleElement
                            {
                                Layout = new ElementLayout().SetFixedWidth(100).SetFixedHeight(50),
                                FillBrush = new ColorBrush(CatTheme.Colors.Primary)
                            },
                            new RectangleElement
                            {
                                Layout = new ElementLayout().SetFixedWidth(30).SetFixedHeight(50),
                                FillBrush = new ColorBrush(CatTheme.Colors.Tertiary)
                            },
                            new RectangleElement
                            {
                                Layout = new ElementLayout().SetFixedWidth(150).SetFixedHeight(50),
                                FillBrush = new ColorBrush(CatTheme.Colors.Primary)
                            },
                            new RectangleElement
                            {
                                Layout = new ElementLayout().SetFixedWidth(80).SetFixedHeight(50),
                                FillBrush = new ColorBrush(CatTheme.Colors.Tertiary)
                            }
                        ]
                    }
                ]
            };
        }

        private static ColumnContainer GetSpacingRowContainerExample(LinearArrangement.JustificationType justification)
        {
            return new ColumnContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%"),
                Arrangement = LinearArrangement.SpacedBy(10),
                Children =
                [
                    new TextBlock($"Alignment: {justification}")
                    {
                        FontSize = 16, TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                    },
                    new RowContainer
                    {
                        Layout = new ElementLayout().SetFixedWidth("100%"),
                        Arrangement = new LinearArrangement(justification, 0),
                        Background = new ColorBrush(CatTheme.Colors.SurfaceContainer),
                        Children =
                        [
                            new RectangleElement
                            {
                                Layout = new ElementLayout().SetFixedWidth(80).SetFixedHeight(50),
                                FillBrush = new ColorBrush(CatTheme.Colors.Primary)
                            },
                            new RectangleElement
                            {
                                Layout = new ElementLayout().SetFixedWidth(80).SetFixedHeight(50),
                                FillBrush = new ColorBrush(CatTheme.Colors.Primary)
                            },
                            new RectangleElement
                            {
                                Layout = new ElementLayout().SetFixedWidth(80).SetFixedHeight(50),
                                FillBrush = new ColorBrush(CatTheme.Colors.Primary)
                            }
                        ]
                    }
                ]
            };
        }
    }
}
