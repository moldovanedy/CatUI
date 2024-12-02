using CatUI.Data.Enums;

namespace CatUI.Data.Containers
{
    public class HBoxContainerSizing : ContainerSizing
    {
        /// <summary>
        /// Controls the growth factor of the element. The growth factor is the portion of the HBoxContainer that is allocated to
        /// this element that is left after all non-growing elements's size (the ones with this property set to 0, the default value)
        /// are subtracted from the total width of the HBoxContainer. See remarks for more details.
        /// This takes precedence over the element's width as long as this is not 0 (in which case the width will be considered).
        /// </summary>
        /// <remarks>
        /// The remaining "unallocated space" of the HBoxContainer is its width without the widths of non-growing element's size.
        /// All the growing elements' HorizontalGrow are summed, then each element gets the corresponding amount of space.
        /// </remarks>
        /// <example>
        /// HBoxContainer has a width of 1000dp. The summed widths of non-growing elements is 400dp, meaning the HBoxContainer's
        /// unallocated space is 600dp. There are 3 growing elements, two with HorizontalGrow of 1 and one with HorizontalGrow of 2.
        /// A "sector" is 150dp (600 / 4), as the sum of the growth factors is 4.
        /// The elements with the growth factor of 1 will have 150dp each (150 * 1), while the other element will have 300dp (150 * 2).
        /// </example>
        public float HGrowthFactor { get; set; }
        public VerticalAlignmentType VerticalAlignment { get; set; } = VerticalAlignmentType.Stretch;

        public HBoxContainerSizing(
            float hGrowthFactor = 1,
            VerticalAlignmentType verticalAlignment = VerticalAlignmentType.Stretch)
        {
            HGrowthFactor = hGrowthFactor;
            VerticalAlignment = verticalAlignment;
        }
    }
}