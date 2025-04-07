namespace CatUI.Data.Theming.Typography
{
    /// <summary>
    /// The typography presets that you can use in your application. These are not used by default by elements, so you
    /// are not forced to use them. By default, all presets have regular weight (400) (except Label, which have 500),
    /// 1.2 line height and the font sizes are different for each preset.
    /// </summary>
    /// <remarks>
    /// These are heavily inspired by Material Design 3 by Google, except that it doesn't have Title. Use of these
    /// properties does not imply any kind of link to Material Design 3, as these are just some presets in the end.
    /// Use of Material Design 3 is not forced by CatUI, nor is it discouraged or encouraged.
    /// </remarks>
    public class CatThemeTypography
    {
        public CatThemeTypography()
        {
            DisplayLargeProperty.ValueChangedEvent += SetDisplayLarge;
            DisplayMediumProperty.ValueChangedEvent += SetDisplayMedium;
            DisplaySmallProperty.ValueChangedEvent += SetDisplaySmall;

            HeadingLargeProperty.ValueChangedEvent += SetHeadingLarge;
            HeadingMediumProperty.ValueChangedEvent += SetHeadingMedium;
            HeadingSmallProperty.ValueChangedEvent += SetHeadingSmall;

            BodyLargeProperty.ValueChangedEvent += SetBodyLarge;
            BodyMediumProperty.ValueChangedEvent += SetBodyMedium;
            BodySmallProperty.ValueChangedEvent += SetBodySmall;

            LabelLargeProperty.ValueChangedEvent += SetLabelLarge;
            LabelMediumProperty.ValueChangedEvent += SetLabelMedium;
            LabelSmallProperty.ValueChangedEvent += SetLabelSmall;
        }

        ~CatThemeTypography()
        {
            DisplayLargeProperty = null!;
            DisplayMediumProperty = null!;
            DisplaySmallProperty = null!;

            HeadingLargeProperty = null!;
            HeadingMediumProperty = null!;
            HeadingSmallProperty = null!;

            BodyLargeProperty = null!;
            BodyMediumProperty = null!;
            BodySmallProperty = null!;

            LabelLargeProperty = null!;
            LabelMediumProperty = null!;
            LabelSmallProperty = null!;
        }

        #region Display

        /// <summary>
        /// "Display" token is generally used for the most important, often short text that immediately gets the user
        /// attention.
        /// </summary>
        public ThemeTextStyle DisplayLarge
        {
            get => _displayLarge;
            internal set
            {
                SetDisplayLarge(value);
                DisplayLargeProperty.Value = value;
            }
        }

        private ThemeTextStyle _displayLarge = new(FontWeightPresets.NORMAL, 64f, 1.2f);

        public ObservableProperty<ThemeTextStyle> DisplayLargeProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 64f, 1.2f));

        private void SetDisplayLarge(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 64f, 1.2f);
            _displayLarge = value;
        }

        /// <summary>
        /// "Display" token is generally used for the most important, often short text that immediately gets the user
        /// attention.
        /// </summary>
        public ThemeTextStyle DisplayMedium
        {
            get => _displayMedium;
            internal set
            {
                SetDisplayMedium(value);
                DisplayMediumProperty.Value = value;
            }
        }

        private ThemeTextStyle _displayMedium = new(FontWeightPresets.NORMAL, 51f, 1.2f);

        public ObservableProperty<ThemeTextStyle> DisplayMediumProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 51f, 1.2f));

        private void SetDisplayMedium(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 51f, 1.2f);
            _displayMedium = value;
        }

        /// <summary>
        /// "Display" token is generally used for the most important, often short text that immediately gets the user
        /// attention.
        /// </summary>
        public ThemeTextStyle DisplaySmall
        {
            get => _displaySmall;
            internal set
            {
                SetDisplaySmall(value);
                DisplaySmallProperty.Value = value;
            }
        }

        private ThemeTextStyle _displaySmall = new(FontWeightPresets.NORMAL, 40f, 1.2f);

        public ObservableProperty<ThemeTextStyle> DisplaySmallProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 40f, 1.2f));

        private void SetDisplaySmall(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 40f, 1.2f);
            _displaySmall = value;
        }

        #endregion

        #region Heading

        /// <summary>
        /// "Heading" token is generally used for titles and important text.
        /// </summary>
        public ThemeTextStyle HeadingLarge
        {
            get => _headingLarge;
            internal set
            {
                SetHeadingLarge(value);
                HeadingLargeProperty.Value = value;
            }
        }

        private ThemeTextStyle _headingLarge = new(FontWeightPresets.NORMAL, 32f, 1.2f);

        public ObservableProperty<ThemeTextStyle> HeadingLargeProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 32f, 1.2f));

        private void SetHeadingLarge(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 32f, 1.2f);
            _headingLarge = value;
        }

        /// <summary>
        /// "Heading" token is generally used for titles and important text.
        /// </summary>
        public ThemeTextStyle HeadingMedium
        {
            get => _headingMedium;
            internal set
            {
                SetHeadingMedium(value);
                HeadingMediumProperty.Value = value;
            }
        }

        private ThemeTextStyle _headingMedium = new(FontWeightPresets.NORMAL, 28f, 1.2f);

        public ObservableProperty<ThemeTextStyle> HeadingMediumProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 28f, 1.2f));

        private void SetHeadingMedium(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 28f, 1.2f);
            _headingMedium = value;
        }

        /// <summary>
        /// "Heading" token is generally used for titles and important text.
        /// </summary>
        public ThemeTextStyle HeadingSmall
        {
            get => _headingSmall;
            internal set
            {
                SetHeadingSmall(value);
                HeadingSmallProperty.Value = value;
            }
        }

        private ThemeTextStyle _headingSmall = new(FontWeightPresets.NORMAL, 25f, 1.2f);

        public ObservableProperty<ThemeTextStyle> HeadingSmallProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 25f, 1.2f));

        private void SetHeadingSmall(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 25f, 1.2f);
            _headingSmall = value;
        }

        #endregion

        #region Body

        /// <summary>
        /// "Body" token is used for most application text.
        /// </summary>
        public ThemeTextStyle BodyLarge
        {
            get => _bodyLarge;
            internal set
            {
                SetBodyLarge(value);
                BodyLargeProperty.Value = value;
            }
        }

        private ThemeTextStyle _bodyLarge = new(FontWeightPresets.NORMAL, 20f, 1.2f);

        public ObservableProperty<ThemeTextStyle> BodyLargeProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 20f, 1.2f));

        private void SetBodyLarge(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 20f, 1.2f);
            _bodyLarge = value;
        }

        /// <summary>
        /// "Body" token is used for most application text.
        /// </summary>
        public ThemeTextStyle BodyMedium
        {
            get => _bodyMedium;
            internal set
            {
                SetBodyMedium(value);
                BodyMediumProperty.Value = value;
            }
        }

        private ThemeTextStyle _bodyMedium = new(FontWeightPresets.NORMAL, 16f, 1.2f);

        public ObservableProperty<ThemeTextStyle> BodyMediumProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 16f, 1.2f));

        private void SetBodyMedium(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 16f, 1.2f);
            _bodyMedium = value;
        }

        /// <summary>
        /// "Body" token is used for most application text.
        /// </summary>
        public ThemeTextStyle BodySmall
        {
            get => _bodySmall;
            internal set
            {
                SetBodySmall(value);
                BodySmallProperty.Value = value;
            }
        }

        private ThemeTextStyle _bodySmall = new(FontWeightPresets.NORMAL, 14f, 1.2f);

        public ObservableProperty<ThemeTextStyle> BodySmallProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.NORMAL, 14f, 1.2f));

        private void SetBodySmall(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.NORMAL, 14f, 1.2f);
            _bodySmall = value;
        }

        #endregion

        #region Label

        /// <summary>
        /// "Label" token is generally used for form-like UI elements (e.g. buttons, check boxes) and less important text.
        /// </summary>
        public ThemeTextStyle LabelLarge
        {
            get => _labelLarge;
            internal set
            {
                SetLabelLarge(value);
                LabelLargeProperty.Value = value;
            }
        }

        private ThemeTextStyle _labelLarge = new(FontWeightPresets.MEDIUM, 14f, 1.2f);

        public ObservableProperty<ThemeTextStyle> LabelLargeProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.MEDIUM, 14f, 1.2f));

        private void SetLabelLarge(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.MEDIUM, 14f, 1.2f);
            _labelLarge = value;
        }

        /// <summary>
        /// "Label" token is generally used for form-like UI elements (e.g. buttons, check boxes) and less important text.
        /// </summary>
        public ThemeTextStyle LabelMedium
        {
            get => _labelMedium;
            internal set
            {
                SetLabelMedium(value);
                LabelMediumProperty.Value = value;
            }
        }

        private ThemeTextStyle _labelMedium = new(FontWeightPresets.MEDIUM, 12f, 1.2f);

        public ObservableProperty<ThemeTextStyle> LabelMediumProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.MEDIUM, 12f, 1.2f));

        private void SetLabelMedium(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.MEDIUM, 12f, 1.2f);
            _labelMedium = value;
        }

        /// <summary>
        /// "Label" token is generally used for form-like UI elements (e.g. buttons, check boxes) and less important text.
        /// </summary>
        public ThemeTextStyle LabelSmall
        {
            get => _labelSmall;
            internal set
            {
                SetLabelSmall(value);
                LabelSmallProperty.Value = value;
            }
        }

        private ThemeTextStyle _labelSmall = new(FontWeightPresets.MEDIUM, 11f, 1.2f);

        public ObservableProperty<ThemeTextStyle> LabelSmallProperty { get; private set; } =
            new(new ThemeTextStyle(FontWeightPresets.MEDIUM, 11f, 1.2f));

        private void SetLabelSmall(ThemeTextStyle? value)
        {
            value ??= new ThemeTextStyle(FontWeightPresets.MEDIUM, 11f, 1.2f);
            _labelSmall = value;
        }

        #endregion
    }
}
