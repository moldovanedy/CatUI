using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Events.Input.Gestures;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Shapes;
using CatUI.Elements.Behaviors;
using CatUI.Utils;

namespace CatUI.Elements.Buttons
{
    public class BaseButton : Element, IClickable, IFocusable
    {
        public const string STATE_DISABLED = "disabled";

        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<BaseButton>? Ref
        {
            get => _ref;
            set
            {
                _ref = value;
                if (_ref != null)
                {
                    _ref.Value = this;
                }
            }
        }

        private ObjectRef<BaseButton>? _ref;

        public bool CanUserCancelClick
        {
            get => _canUserCancelClick;
            set => CanUserCancelClickProperty.Value = value;
        }

        private bool _canUserCancelClick = true;
        public ObservableProperty<bool> CanUserCancelClickProperty { get; } = new(true);

        private void SetCanUserCancelClick(bool value)
        {
            _canUserCancelClick = value;
            SetLocalValue(nameof(CanUserCancelClick), value);
        }

        public event ClickEventHandler? ClickEvent;

        public ClickEventHandler? OnClick
        {
            get => _onClick;
            set
            {
                ClickEvent -= _onClick;
                _onClick = value;
                ClickEvent += _onClick;
            }
        }

        private ClickEventHandler? _onClick;


        #region Focus

        public IFocusable? NextFocusableElement { get; set; }
        public IFocusable? PreviousFocusableElement { get; set; }

        public void OnFocusStateChanged(bool hasEnteredFocus)
        {
            FocusChangedEvent?.Invoke(this, hasEnteredFocus);
        }

        public bool IsFocusEnabled
        {
            get => _isFocusEnabled;
            set => IsFocusEnabledProperty.Value = value;
        }

        private bool _isFocusEnabled = true;
        public ObservableProperty<bool> IsFocusEnabledProperty { get; } = new(true);

        private void SetIsFocusEnabled(bool value)
        {
            _isFocusEnabled = value;
            SetLocalValue(nameof(IsFocusEnabled), value);
        }

        public event FocusChangedEventHandler? FocusChangedEvent;

        public FocusChangedEventHandler? OnFocusChanged
        {
            get => _onFocusChanged;
            set
            {
                FocusChangedEvent -= _onFocusChanged;
                _onFocusChanged = value;
                FocusChangedEvent += _onFocusChanged;
            }
        }

        private FocusChangedEventHandler? _onFocusChanged;

        #endregion

        private bool _isDown;

        public BaseButton()
        {
            CanUserCancelClickProperty.ValueChangedEvent += SetCanUserCancelClick;
            IsFocusEnabledProperty.ValueChangedEvent += SetIsFocusEnabled;
            PointerDownEvent += PrivatePointerDown;
            PointerUpEvent += PrivatePointerUp;
            ClickEvent += PrivateClick;
        }

        public virtual void Click(object sender, ClickEventArgs e) { }
        public void FocusChanged(object sender, FocusChangedEventHandler e) { }

        public override Element Duplicate()
        {
            BaseButton el = new()
            {
                CanUserCancelClick = CanUserCancelClick,
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                LocallyVisible = LocallyVisible,
                LocallyEnabled = LocallyEnabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };

            DuplicateChildrenUtil(el);
            return el;
        }

        private void PrivatePointerDown(object sender, PointerDownEventArgs e)
        {
            _isDown = true;
        }

        private void PrivatePointerUp(object sender, PointerUpEventArgs e)
        {
            if (!_isDown)
            {
                return;
            }

            _isDown = false;
            if (e.WasCancelled && CanUserCancelClick)
            {
                return;
            }

            ClickEvent?.Invoke(this, new ClickEventArgs(e.Position, e.AbsolutePosition));
        }

        private void PrivateClick(object sender, ClickEventArgs e)
        {
            this.GrabFocus();
        }
    }
}
