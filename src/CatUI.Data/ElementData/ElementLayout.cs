namespace CatUI.Data.ElementData
{
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
            private set => _layoutFlags |= (byte)((byte)value & 0b11);
        }

        public LayoutMode HeightMode
        {
            get => (LayoutMode)((_layoutFlags & 0b11000) >> 3);
            private set => _layoutFlags |= (byte)(((byte)value & 0b11) << 3);
        }

        /// <summary>
        /// If true when <see cref="WidthMode"/> is <see cref="LayoutMode.MinMax"/>, the element will try to stretch
        /// until <see cref="MaxWidth"/> is reached. If <see cref="MaxWidth"/> is not set, the maximum width will be
        /// treated as +infinity, so <see cref="GetSuggestedWidth"/> will return <see cref="float.PositiveInfinity"/>.
        /// </summary>
        public bool PrefersMaxWidth
        {
            get => (_layoutFlags & 0b100) != 0;
            private set
            {
                if (value)
                {
                    _layoutFlags |= 0b100;
                }
                else
                {
                    _layoutFlags &= ~0b100 & 0xff;
                }
            }
        }

        /// <summary>
        /// If true when <see cref="HeightMode"/> is <see cref="LayoutMode.MinMax"/>, the element will try to stretch
        /// until <see cref="MaxHeight"/> is reached. If <see cref="MaxHeight"/> is not set, the maximum height will be
        /// treated as +infinity, so <see cref="GetSuggestedHeight"/> will return <see cref="float.PositiveInfinity"/>.
        /// </summary>
        public bool PrefersMaxHeight
        {
            get => (_layoutFlags & (1 << 5)) != 0;
            private set
            {
                if (value)
                {
                    _layoutFlags |= 1 << 5;
                }
                else
                {
                    _layoutFlags &= ~(1 << 5) & 0xff;
                }
            }
        }

        private byte _layoutFlags;

        #region Modifiers

        /// <summary>
        /// Sets the given width as a "fixed" width, meaning the element won't be able to stretch or shrink, it must
        /// respect this width. This will set <see cref="WidthMode"/> to <see cref="LayoutMode.Fixed"/>.
        /// </summary>
        /// <remarks>
        /// This will reset all the other values related to width (<see cref="MinWidth"/>, <see cref="MaxWidth"/>
        /// and <see cref="PrefersMaxWidth"/>).
        /// </remarks>
        /// <param name="width">The width that you want the element to be fixed at.</param>
        /// <returns>This instance (to make an element hierarchy setup easier).</returns>
        public ElementLayout SetFixedWidth(Dimension width)
        {
            WidthMode = LayoutMode.Fixed;
            PreferredWidth = width;

            MinWidth = null;
            MaxWidth = null;
            PrefersMaxWidth = false;
            return this;
        }

        /// <summary>
        /// Sets the given height as a "fixed" height, meaning the element won't be able to stretch or shrink, it must
        /// respect this height. This will set <see cref="HeightMode"/> to <see cref="LayoutMode.Fixed"/>.
        /// </summary>
        /// <remarks>
        /// This will reset all the other values related to height (<see cref="MinHeight"/>, <see cref="MaxHeight"/>
        /// and <see cref="PrefersMaxHeight"/>).
        /// </remarks>
        /// <param name="height">The height that you want the element to be fixed at.</param>
        /// <returns>This instance (to make an element hierarchy setup easier).</returns>
        public ElementLayout SetFixedHeight(Dimension height)
        {
            HeightMode = LayoutMode.Fixed;
            PreferredHeight = height;

            MinHeight = null;
            MaxHeight = null;
            PrefersMaxHeight = false;
            return this;
        }

        /// <summary>
        /// Sets the minimum and maximum width constraints, the element will be able to have any size between these
        /// limits. This will set <see cref="WidthMode"/> to <see cref="LayoutMode.MinMax"/>.
        /// </summary>
        /// <remarks>This will reset <see cref="PreferredWidth"/> as it's irrelevant.</remarks>
        /// <param name="minWidth">The minimum width the element is allowed to have.</param>
        /// <param name="maxWidth">The maximum width the element is allowed to have.</param>
        /// <param name="prefersMaxWidth">
        /// If set to a value, it will set <see cref="PrefersMaxWidth"/> to that value.
        /// </param>
        /// <returns>This instance (to make an element hierarchy setup easier).</returns>
        public ElementLayout SetMinMaxWidth(Dimension minWidth, Dimension maxWidth, bool? prefersMaxWidth = null)
        {
            WidthMode = LayoutMode.MinMax;
            MinWidth = minWidth;
            MaxWidth = maxWidth;

            if (prefersMaxWidth != null)
            {
                PrefersMaxWidth = prefersMaxWidth.Value;
            }

            PreferredWidth = null;
            return this;
        }

        /// <summary>
        /// Sets the minimum and maximum height constraints, the element will be able to have any size between these
        /// limits. This will set <see cref="HeightMode"/> to <see cref="LayoutMode.MinMax"/>.
        /// </summary>
        /// <remarks>This will reset <see cref="PrefersMaxWidth"/> as it's irrelevant.</remarks>
        /// <param name="minHeight">The minimum height the element is allowed to have.</param>
        /// <param name="maxHeight">The maximum height the element is allowed to have.</param>
        /// <param name="prefersMaxHeight">
        /// If set to a value, it will set <see cref="PrefersMaxHeight"/> to that value.
        /// </param>
        /// <returns>This instance (to make an element hierarchy setup easier).</returns>
        public ElementLayout SetMinMaxHeight(Dimension minHeight, Dimension maxHeight, bool? prefersMaxHeight = null)
        {
            HeightMode = LayoutMode.MinMax;
            MinHeight = minHeight;
            MaxHeight = maxHeight;

            if (prefersMaxHeight != null)
            {
                PrefersMaxHeight = prefersMaxHeight.Value;
            }

            PreferredHeight = null;
            return this;
        }

        /// <summary>
        /// Sets the preferred width, as well as the minimum and maximum constraints, meaning the element will prefer
        /// that width, but will be able to freely stretch or shrink based on the given limits if needed.
        /// This will set <see cref="WidthMode"/> to <see cref="LayoutMode.MinMaxAndPreferred"/>.
        /// </summary>
        /// <remarks>Will set <see cref="PrefersMaxWidth"/> to false as it's irrelevant here.</remarks>
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

            PrefersMaxWidth = false;
            return this;
        }

        /// <summary>
        /// Sets the preferred height, as well as the minimum and maximum constraints, meaning the element will prefer
        /// that height, but will be able to freely stretch or shrink based on the given limits if needed.
        /// This will set <see cref="HeightMode"/> to <see cref="LayoutMode.MinMaxAndPreferred"/>.
        /// </summary>
        /// <remarks>Will set <see cref="PrefersMaxHeight"/> to false as it's irrelevant here.</remarks>
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

            PrefersMaxHeight = false;
            return this;
        }

        /// <summary>
        /// Will try to set <see cref="PrefersMaxWidth"/>, but will only modify it if <see cref="WidthMode"/> is
        /// <see cref="LayoutMode.MinMax"/>, otherwise no change will occur and false will be returned.
        /// </summary>
        /// <param name="prefersMaxWidth">The desired value for <see cref="PrefersMaxWidth"/>.</param>
        /// <returns>
        /// True if the operation succeeded (when <see cref="WidthMode"/> is <see cref="LayoutMode.MinMax"/>),
        /// false otherwise.
        /// </returns>
        public bool SetPrefersMaxWidth(bool prefersMaxWidth)
        {
            if (WidthMode == LayoutMode.MinMax)
            {
                PrefersMaxWidth = prefersMaxWidth;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Will try to set <see cref="PrefersMaxHeight"/>, but will only modify it if <see cref="HeightMode"/> is
        /// <see cref="LayoutMode.MinMax"/>, otherwise no change will occur and false will be returned.
        /// </summary>
        /// <param name="prefersMaxHeight">The desired value for <see cref="PrefersMaxHeight"/>.</param>
        /// <returns>
        /// True if the operation succeeded (when <see cref="HeightMode"/> is <see cref="LayoutMode.MinMax"/>),
        /// false otherwise.
        /// </returns>
        public bool SetPrefersMaxHeight(bool prefersMaxHeight)
        {
            if (HeightMode == LayoutMode.MinMax)
            {
                PrefersMaxHeight = prefersMaxHeight;
                return true;
            }

            return false;
        }

        #endregion //Modifiers

        public Dimension? GetSuggestedWidth()
        {
            switch (WidthMode)
            {
                case LayoutMode.MinMaxAndPreferred:
                case LayoutMode.Fixed:
                    return PreferredWidth;
                case LayoutMode.MinMax:
                    return
                        PrefersMaxWidth
                            ? (MaxWidth ?? Dimension.Unset).IsUnset() ? float.PositiveInfinity : MaxWidth
                            : (MinWidth ?? Dimension.Unset).IsUnset()
                                ? 0
                                : MinWidth;
                default:
                    return null;
            }
        }

        public Dimension? GetSuggestedHeight()
        {
            switch (HeightMode)
            {
                case LayoutMode.MinMaxAndPreferred:
                case LayoutMode.Fixed:
                    return PreferredHeight;
                case LayoutMode.MinMax:
                    return
                        PrefersMaxHeight
                            ? (MaxHeight ?? Dimension.Unset).IsUnset() ? float.PositiveInfinity : MaxHeight
                            : (MinHeight ?? Dimension.Unset).IsUnset()
                                ? 0
                                : MinHeight;
                default:
                    return null;
            }
        }


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
            /// The dimension will be somewhere between the min and max limits, shrinking and stretching allowed.
            /// The element might also be instructed to try to stretch to the maximum dimension instead of the minimum one.
            /// </summary>
            MinMax = 1,

            /// <summary>
            /// The dimension should be the preferred one, but the element is allowed to shrink or stretch in regard
            /// to the minimum and maximum dimensions.
            /// </summary>
            MinMaxAndPreferred = 2
        }
    }
}
