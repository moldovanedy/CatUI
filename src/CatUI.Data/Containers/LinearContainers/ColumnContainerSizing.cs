using System.ComponentModel;
using System.Runtime.CompilerServices;
using CatUI.Data.Enums;

namespace CatUI.Data.Containers.LinearContainers
{
    public class ColumnContainerSizing : ContainerSizing, INotifyPropertyChanged
    {
        /// <summary>
        /// Controls the growth factor of the element. The growth factor is the portion of the ColumnContainer that is
        /// allocated to this element that is left after all non-growing elements' minimum size (the ones with this
        /// property set to 0, the default value) are subtracted from the total height of the ColumnContainer. See
        /// remarks for more details. This takes precedence over the element's preferred height as long as this is
        /// larger than 0 (in which case the behavior will be the same as a normal, non-growing element).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The remaining "unallocated space" of the ColumnContainer is its height without the heights of non-growing
        /// element's size. All the growing elements' GrowthFactor are summed, then each element gets the corresponding
        /// amount of space.
        /// </para>
        /// <para>
        /// NOTE: When this is larger than 0, the element's preferred height will be ignored, but the minimum and maximum
        /// heights will be considered. When this is 0 (or lower, making no sense), this is ignored and the element
        /// will behave the same as a non-growing element.
        /// </para>
        /// </remarks>
        /// <example>
        /// ColumnContainer has a height of 1000dp. The summed heights of non-growing elements is 400dp, meaning the
        /// ColumnContainer's unallocated space is 600dp. There are 3 growing elements, two with GrowthFactor of 1
        /// and one with GrowthFactor of 2. A "sector" is 150dp (600 / 4), as the sum of the growth factors is 4.
        /// The elements with the growth factor of 1 will have 150dp each (150 * 1), while the other element will have
        /// 300dp (150 * 2).
        /// </example>
        public float GrowthFactor
        {
            get => _growthFactor;
            set
            {
                _growthFactor = value;
                NotifyPropertyChanged();
            }
        }

        private float _growthFactor;

        public HorizontalAlignmentType HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                _horizontalAlignment = value;
                NotifyPropertyChanged();
            }
        }

        private HorizontalAlignmentType _horizontalAlignment;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ColumnContainerSizing(
            float growthFactor = 0,
            HorizontalAlignmentType horizontalAlignment = HorizontalAlignmentType.Left)
        {
            GrowthFactor = growthFactor;
            HorizontalAlignment = horizontalAlignment;
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public override ColumnContainerSizing Duplicate()
        {
            return new ColumnContainerSizing(GrowthFactor, HorizontalAlignment);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
