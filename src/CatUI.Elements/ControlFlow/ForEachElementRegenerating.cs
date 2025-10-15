using CatUI.Utils;

namespace CatUI.Elements.ControlFlow;

/// <summary>
/// The same as <see cref="ForEachElement{T}"/>, but on any change to the given item collection (add, update, remove,
/// move), the whole elements are cleared and the generator function is run again for each element of the list.
/// </summary>
/// <typeparam name="T">The data type of items in the collection.</typeparam>
public class ForEachElementRegenerating<T> : ForEachElement<T>
{
    /// <inheritdoc cref="Element.Ref"/>
    public new ObjectRef<ForEachElementRegenerating<T>>? Ref
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

    private ObjectRef<ForEachElementRegenerating<T>>? _ref;

    public ForEachElementRegenerating(
        Element generatorParent,
        ObservableList<T> items,
        GeneratorFunctionCallback generatorFunction)
        : base(generatorParent, items, generatorFunction)
    {
    }

    public ForEachElementRegenerating(ForEachElementRegenerating<T> other) : base(other)
    {
    }

    protected sealed override void OnItemRemoved(object? sender, ObservableListRemoveEventArgs<T> e)
    {
        RegenerateElements();
    }

    protected sealed override void OnItemInserted(object? sender, ObservableListInsertEventArgs<T> e)
    {
        RegenerateElements();
    }

    protected sealed override void OnItemMoved(object? sender, ObservableListMoveEventArgs<T> e)
    {
        RegenerateElements();
    }

    private void RegenerateElements()
    {
        GeneratorParent.Children.Clear();
        foreach (T item in Items)
        {
            GeneratorParent.Children.Add(GeneratorFunction.Invoke(Items.IndexOf(item), item));
        }
    }

    public override ForEachElementRegenerating<T> Duplicate()
    {
        ForEachElementRegenerating<T> el = new(this);
        DuplicateChildrenUtil(el);
        return el;
    }
}
