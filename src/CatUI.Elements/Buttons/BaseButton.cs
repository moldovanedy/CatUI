using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Events.Input.Gestures;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Shapes;
using CatUI.Elements.Behaviors;
using CatUI.Utils;

namespace CatUI.Elements.Buttons
{
    public class BaseButton : Element, IClickable
    {
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
            set
            {
                SetCanUserCancelClick(value);
                CanUserCancelClickProperty.Value = value;
            }
        }

        private bool _canUserCancelClick = true;
        public ObservableProperty<bool> CanUserCancelClickProperty { get; } = new(true);

        private void SetCanUserCancelClick(bool value)
        {
            _canUserCancelClick = value;
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

        private bool _isDown;

        public BaseButton()
        {
            CanUserCancelClickProperty.ValueChangedEvent += SetCanUserCancelClick;
            PointerDownEvent += PrivatePointerDown;
            PointerUpEvent += PrivatePointerUp;
        }

        //~BaseButton()
        //{
        //    ClickEvent = null;
        //    CanUserCancelClickProperty = null!;

        //    PointerDownEvent -= PrivatePointerDown;
        //    PointerUpEvent -= PrivatePointerUp;
        //}

        public virtual void Click(object sender, ClickEventArgs e) { }

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
                Visible = Visible,
                Enabled = Enabled,
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
    }
}
