using System.ComponentModel;
using System.Runtime.CompilerServices;
using CatUI.Utils;

namespace CatUI.Data.ElementData;

/// <summary>
/// Represents the element's layout description. By default, both the width and the height use
/// <see cref="LayoutMode.MinMaxAndPreferred"/>, trying to respect the minimum (0), while the maximum is unset
/// (infinity) and is only constrained by the parent's max size. 
/// </summary>
/// <remarks>
/// Although it implements <see cref="INotifyPropertyChanged"/>, it won't actually fire <see cref="PropertyChanged"/>
/// for each property, rather it will fire when using one of the Set* methods (i.e. <see cref="SetFixedHeight"/> and
/// <see cref="SetMinMaxAndPreferredHeight"/>).
/// </remarks>
public class ElementLayout
{
    public Dimension? PreferredWidth { get; private set; }
    public Dimension? PreferredHeight { get; private set; }

    public Dimension? MinWidth { get; private set; }
    public Dimension? MinHeight { get; private set; }

    public Dimension? MaxWidth { get; private set; }
    public Dimension? MaxHeight { get; private set; }

    public LayoutMode WidthMode
    {
        get => (LayoutMode)(_layoutFlags & 0b11);
        private set
        {
            bool lowBit = ((int)value & 0b1) != 0;
            bool highBit = ((int)value & 0b10) != 0;
            int flags = _layoutFlags;

            BinaryUtils.SetBit(ref flags, lowBit, 0);
            BinaryUtils.SetBit(ref flags, highBit, 1);

            _layoutFlags = (byte)flags;
        }
    }

    public LayoutMode HeightMode
    {
        get => (LayoutMode)((_layoutFlags & 0b11000) >> 3);
        private set
        {
            bool lowBit = ((int)value & 0b1) != 0;
            bool highBit = ((int)value & 0b10) != 0;
            int flags = _layoutFlags;

            BinaryUtils.SetBit(ref flags, lowBit, 3);
            BinaryUtils.SetBit(ref flags, highBit, 4);

            _layoutFlags = (byte)flags;
        }
    }

    //default as MinMax for both Width and Height
    private byte _layoutFlags;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ElementLayout()
    {
        WidthMode = LayoutMode.MinMaxAndPreferred;
        HeightMode = LayoutMode.MinMaxAndPreferred;
    }

    #region Modifiers

    /// <summary>
    /// Sets the given width as a "fixed" width, meaning the element won't be able to stretch or shrink, it must
    /// respect this width. This will set <see cref="WidthMode"/> to <see cref="LayoutMode.Fixed"/>.
    /// </summary>
    /// <remarks>
    /// This will reset all the other values related to width (<see cref="MinWidth"/> and <see cref="MaxWidth"/>).
    /// </remarks>
    /// <param name="width">The width that you want the element to be fixed at.</param>
    /// <returns>This instance (to make an element hierarchy setup easier).</returns>
    public ElementLayout SetFixedWidth(Dimension width)
    {
        WidthMode = LayoutMode.Fixed;
        PreferredWidth = width;

        MinWidth = null;
        MaxWidth = null;
        NotifyPropertyChanged();
        return this;
    }

    /// <summary>
    /// Sets the given height as a "fixed" height, meaning the element won't be able to stretch or shrink, it must
    /// respect this height. This will set <see cref="HeightMode"/> to <see cref="LayoutMode.Fixed"/>.
    /// </summary>
    /// <remarks>
    /// This will reset all the other values related to height (<see cref="MinHeight"/> and <see cref="MaxHeight"/>).
    /// </remarks>
    /// <param name="height">The height that you want the element to be fixed at.</param>
    /// <returns>This instance (to make an element hierarchy setup easier).</returns>
    public ElementLayout SetFixedHeight(Dimension height)
    {
        HeightMode = LayoutMode.Fixed;
        PreferredHeight = height;

        MinHeight = null;
        MaxHeight = null;
        NotifyPropertyChanged();
        return this;
    }

    /// <summary>
    /// Sets the preferred width, as well as the minimum and maximum constraints, meaning the element will prefer
    /// that width, but will be able to freely stretch or shrink based on the given limits if needed.
    /// This will set <see cref="WidthMode"/> to <see cref="LayoutMode.MinMaxAndPreferred"/>.
    /// </summary>
    /// <param name="prefWidth">The preferred width the element will try to respect.</param>
    /// <param name="minWidth">The minimum width the element is allowed to have.</param>
    /// <param name="maxWidth">The maximum width the element is allowed to have.</param>
    /// <returns>This instance (to make an element hierarchy setup easier).</returns>
    public ElementLayout SetMinMaxAndPreferredWidth(Dimension prefWidth, Dimension minWidth, Dimension maxWidth)
    {
        WidthMode = LayoutMode.MinMaxAndPreferred;
        PreferredWidth = prefWidth;
        MinWidth = minWidth;
        MaxWidth = maxWidth;

        NotifyPropertyChanged();
        return this;
    }

    /// <summary>
    /// Sets the preferred height, as well as the minimum and maximum constraints, meaning the element will prefer
    /// that height, but will be able to freely stretch or shrink based on the given limits if needed.
    /// This will set <see cref="HeightMode"/> to <see cref="LayoutMode.MinMaxAndPreferred"/>.
    /// </summary>
    /// <param name="prefHeight">The preferred height the element will try to respect.</param>
    /// <param name="minHeight">The minimum height the element is allowed to have.</param>
    /// <param name="maxHeight">The maximum height the element is allowed to have.</param>
    /// <returns>This instance (to make an element hierarchy setup easier).</returns>
    public ElementLayout SetMinMaxAndPreferredHeight(Dimension prefHeight, Dimension minHeight, Dimension maxHeight)
    {
        HeightMode = LayoutMode.MinMaxAndPreferred;
        PreferredHeight = prefHeight;
        MinHeight = minHeight;
        MaxHeight = maxHeight;

        NotifyPropertyChanged();
        return this;
    }

    #endregion //Modifiers


    /// <summary>
    /// Represents the mode that the width or height of an element will respect.
    /// </summary>
    public enum LayoutMode
    {
        /// <summary>
        /// The set dimension will always be respected, no shrinking or stretching allowed.
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// The dimension should be the preferred one, but the element is allowed to shrink or stretch in regard
        /// to the minimum and maximum dimensions.
        /// </summary>
        MinMaxAndPreferred = 2
    }
}
