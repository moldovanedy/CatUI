using System;
using CatUI.Data;
using CatUI.Utils;

namespace CatUI.Elements.ControlFlow;

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
        set => GeneratorParentProperty.Value = value;
    }

    private Element _generatorParent;
    public ObservableProperty<Element> GeneratorParentProperty
    {
        get => _generatorParentProperty;
        set => _generatorParentProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<Element> _generatorParentProperty = new();

    private void SetGeneratorParent(Element? value)
    {
        if (value == null)
        {
            return;
        }

        _generatorParent = value;
        SetLocalValue(nameof(GeneratorParent), value);

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
        Init();
        _start = start;
        _end = end;
        _step = step;
        _callback = callback;

        GeneratorParent = generatorParent;
        //silence compiler
        _generatorParent = generatorParent;

        Reevaluate();
    }

    /// <remarks>Does not clone <see cref="GeneratorParent"/>.</remarks>
    public ForElement(ForElement other) : base(other)
    {
        Init();
        _start = other._start;
        _end = other._end;
        _step = other._step;
        _callback = other._callback;

        GeneratorParent = other._generatorParent;
        //silence compiler
        _generatorParent = other._generatorParent;

        Reevaluate();
    }

    private void Init()
    {
        GeneratorParentProperty.ValueChangedEvent += SetGeneratorParent;
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
        var el = new ForElement(this);
        DuplicateChildrenUtil(el);
        return el;
    }
}
