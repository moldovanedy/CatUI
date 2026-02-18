using System;
using CatUI.Data;
using CatUI.Utils;

namespace CatUI.Elements.ControlFlow;

/// <summary>
/// A control flow element that calls the given callback when the given collection is modified (by having elements
/// added or updated) and attaches the resulting element to the Document. DO NOT directly manipulate children (add,
/// remove), as they are used internally and might cause errors otherwise.
/// </summary>
/// <typeparam name="T">The data type of items in the collection.</typeparam>
public class ForEachElement<T> : ControlFlowElementBase
{
    /// <inheritdoc cref="Element.Ref"/>
    public new ObjectRef<ForEachElement<T>>? Ref
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

    private ObjectRef<ForEachElement<T>>? _ref;

    /// <summary>
    /// Represents the items of the collection you want to iterate through.
    /// </summary>
    public ObservableList<T> Items
    {
        get => _items;
        set
        {
            GeneratorParent.Children.Clear();

            _items = value;
            _items.ItemInsertedEvent += OnItemInserted;
            _items.ItemRemovedEvent += OnItemRemoved;
            _items.ItemMovedEvent += OnItemMoved;
            _items.ListClearedEvent += OnItemListCleared;
        }
    }

    private ObservableList<T> _items = [];

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

    /// <summary>
    /// The function that will generate the element. Will receive the index of the modified item as a parameter.
    /// This only gets called when there is an addition or an update to the collection, not for removals or moves
    /// (that is handled internally).
    /// </summary>
    public GeneratorFunctionCallback GeneratorFunction
    {
        get => _generatorFunction;
        set => GeneratorFunctionProperty.Value = value;
    }

    private GeneratorFunctionCallback _generatorFunction;
    public ObservableProperty<GeneratorFunctionCallback> GeneratorFunctionProperty
    {
        get => _generatorFunctionProperty;
        set => _generatorFunctionProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<GeneratorFunctionCallback> _generatorFunctionProperty = new();

    private void SetGeneratorFunction(GeneratorFunctionCallback? value)
    {
        if (value != null)
        {
            _generatorFunction = value;
            SetLocalValue(nameof(GeneratorFunction), value);
        }
    }

    public ForEachElement(
        Element generatorParent,
        ObservableList<T> items,
        GeneratorFunctionCallback generatorFunction)
    {
        //silence compiler
        _generatorParent = null!;
        _generatorFunction = null!;

        Init(generatorParent, items, generatorFunction);
    }

    /// <remarks>
    /// The <see cref="GeneratorParent"/> and <see cref="GeneratorFunction"/> are not cloned.
    /// </remarks>
    public ForEachElement(ForEachElement<T> other) : base(other)
    {
        //silence compiler
        _generatorParent = null!;
        _generatorFunction = null!;

        ObservableList<T> items = [];
        foreach (T item in other.Items)
        {
            items.Add(item);
        }

        Init(other.GeneratorParent, items, other.GeneratorFunction);
    }

    protected virtual void OnItemRemoved(object? sender, ObservableListRemoveEventArgs<T> e)
    {
        GeneratorParent.Children.RemoveAt(e.Index);
    }

    protected virtual void OnItemInserted(object? sender, ObservableListInsertEventArgs<T> e)
    {
        GeneratorParent.Children.Insert(e.Index, _generatorFunction.Invoke(e.Index, Items[e.Index]));
    }

    protected virtual void OnItemMoved(object? sender, ObservableListMoveEventArgs<T> e)
    {
        GeneratorParent.Children.Move(Children[e.OldIndex], e.NewIndex);
    }

    private void OnItemListCleared(object? sender, EventArgs e)
    {
        GeneratorParent.Children.Clear();
    }

    /// <remarks>
    /// <see cref="GeneratorFunction"/> is not cloned.
    /// </remarks>
    public override ForEachElement<T> Duplicate()
    {
        ForEachElement<T> el = new(this);
        DuplicateChildrenUtil(el);
        return el;
    }

    private void Init(
        Element generatorParent,
        ObservableList<T> items,
        GeneratorFunctionCallback generatorFunction)
    {
        GeneratorParentProperty.ValueChangedEvent += SetGeneratorParent;
        GeneratorFunctionProperty.ValueChangedEvent += SetGeneratorFunction;

        GeneratorParent = generatorParent;
        GeneratorFunction = generatorFunction;

        Items = items;
        for (int i = 0; i < Items.Count; i++)
        {
            GeneratorParent.Children.Insert(i, _generatorFunction.Invoke(i, Items[i]));
        }
    }


    public delegate Element GeneratorFunctionCallback(int index, T item);
}
