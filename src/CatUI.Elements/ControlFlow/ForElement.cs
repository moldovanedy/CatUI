using System;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.ControlFlow
{
    /// <summary>
    /// A control flow element that attaches the element from the callback to the Document. DO NOT
    /// directly manipulate children (add, remove), as they are used internally and might cause errors otherwise.
    /// </summary>
    public class ForElement : ControlFlowElementBase
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<ForElement>? Ref
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

        private ObjectRef<ForElement>? _ref;

        /// <summary>
        /// The parent element of the generated elements. Not to be confused with this element's parent, which is
        /// obtained using <see cref="Element.GetParent"/>; this is actually the child of the ForEachElement.
        /// DO NOT manipulate children of this element, as they are used internally and might cause errors otherwise.
        /// </summary>
        public Element GeneratorParent
        {
            get => _generatorParent;
            set
            {
                SetGeneratorParent(value);
                GeneratorParentProperty.Value = value;
            }
        }

        private Element _generatorParent;
        public ObservableProperty<Element> GeneratorParentProperty { get; } = new();

        private void SetGeneratorParent(Element? value)
        {
            if (value == null)
            {
                return;
            }

            _generatorParent = value;
            if (Children.Count > 0)
            {
                Children.RemoveAt(0);
            }

            Children.Add(_generatorParent);
        }

        private int _start;
        private readonly int _end;
        private readonly int _step;
        private readonly Func<int, Element> _callback;

        public ForElement(int start, int end, int step, Element generatorParent, Func<int, Element> callback)
        {
            _start = start;
            _end = end;
            _step = step;
            _callback = callback;

            //silence compiler
            _generatorParent = generatorParent;
            GeneratorParent = generatorParent;
            GeneratorParentProperty.ValueChangedEvent += SetGeneratorParent;

            Reevaluate();
        }

        private void Reevaluate()
        {
            int initialStart = _start;

            GeneratorParent.Children.Clear();
            for (; _start < _end; _start += _step)
            {
                GeneratorParent.Children.Add(_callback.Invoke(_start));
            }

            _start = initialStart;
        }

        public override ForElement Duplicate()
        {
            ForElement el = new(_start, _end, _step, _generatorParent.Duplicate(), _callback)
            {
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
    }
}
