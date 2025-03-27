using System.ComponentModel;
using System.Runtime.CompilerServices;
using CatUI.Data.Enums;

namespace CatUI.Data.Containers.LinearContainers
{
    public class RowContainerSizing : ContainerSizing, INotifyPropertyChanged
    {
        /// <summary>
        /// Controls the growth factor of the element. The growth factor is the portion of the RowContainer that is
        /// allocated to this element that is left after all non-growing elements' minimum size (the ones with this
        /// property set to 0, the default value) are subtracted from the total width of the RowContainer. See
        /// remarks for more details. This takes precedence over the element's preferred width as long as this is
        /// larger than 0 (in which case the behavior will be the same as a normal, non-growing element).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The remaining "unallocated space" of the RowContainer is its width without the widths of non-growing element's
        /// minimum size. All the growing elements' GrowthFactor are summed, then each element gets the corresponding
        /// amount of space.
        /// </para>
        /// <para>
        /// NOTE: When this is larger than 0, the element's preferred width will be ignored, but the minimum and maximum
        /// widths will be considered. When this is 0 (or lower, making no sense), this is ignored and the element
        /// will behave the same as a non-growing element.
        /// </para>
        /// </remarks>
        /// <example>
        /// RowContainer has a width of 1000dp. The summed widths of non-growing elements is 400dp, meaning the
        /// RowContainer's unallocated space is 600dp. There are 3 growing elements, two with GrowthFactor of 1
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

        public VerticalAlignmentType VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                _verticalAlignment = value;
                NotifyPropertyChanged();
            }
        }

        private VerticalAlignmentType _verticalAlignment;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RowContainerSizing(
            float growthFactor = 1,
            VerticalAlignmentType verticalAlignment = VerticalAlignmentType.Top)
        {
            GrowthFactor = growthFactor;
            VerticalAlignment = verticalAlignment;
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public override RowContainerSizing Duplicate()
        {
            return new RowContainerSizing(GrowthFactor, VerticalAlignment);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
