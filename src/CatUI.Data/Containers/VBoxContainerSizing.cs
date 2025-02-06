using CatUI.Data.Enums;

namespace CatUI.Data.Containers
{
    public class VBoxContainerSizing : ContainerSizing
    {
        /// <summary>
        /// Controls the growth factor of the element. The growth factor is the portion of the VBoxContainer that is allocated to
        /// this element that is left after all non-growing elements' size (the ones with this property set to 0, the default value)
        /// are subtracted from the total height of the VBoxContainer. See remarks for more details.
        /// This takes precedence over the element's height as long as this is larger than 0
        /// (in which case the height will be considered).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The remaining "unallocated space" of the VBoxContainer is its height without the heights of non-growing element's size.
        /// All the growing elements' VerticalGrow are summed, then each element gets the corresponding amount of space.
        /// </para>
        /// <para>
        /// NOTE: When this is larger than 0, the element's PreferredHeight and MaxHeight will be ignored,
        /// but MinHeight will be considered. When this is 0 (or lower, making no sense), this is ignored and PreferredHeight is considered.
        /// </para>
        /// </remarks>
        /// <example>
        /// VBoxContainer has a height of 1000dp. The summed heights of non-growing elements is 400dp, meaning the VBoxContainer's
        /// unallocated space is 600dp. There are 3 growing elements, two with VerticalGrow of 1 and one with VerticalGrow of 2.
        /// A "sector" is 150dp (600 / 4), as the sum of the growth factors is 4.
        /// The elements with the growth factor of 1 will have 150dp each (150 * 1), while the other element will have 300dp (150 * 2).
        /// </example>
        public float VGrowthFactor { get; set; }

        public HorizontalAlignmentType HorizontalAlignment { get; set; }

        public VBoxContainerSizing(
            float vGrowthFactor = 1,
            HorizontalAlignmentType horizontalAlignment = HorizontalAlignmentType.Stretch)
        {
            VGrowthFactor = vGrowthFactor;
            HorizontalAlignment = horizontalAlignment;
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public override VBoxContainerSizing Duplicate()
        {
            return new VBoxContainerSizing(VGrowthFactor, HorizontalAlignment);
        }
    }
}
