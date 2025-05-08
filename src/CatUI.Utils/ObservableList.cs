using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CatUI.Utils
{
    public class ObservableList<T> : Collection<T>
    {
        private bool _shouldFireEvents = true;

        public event EventHandler<ObservableListInsertEventArgs<T>>? ItemInsertedEvent;
        public event EventHandler<ObservableListRemoveEventArgs<T>>? ItemRemovedEvent;
        public event EventHandler<ObservableListMoveEventArgs<T>>? ItemMovedEvent;
        public event EventHandler<EventArgs>? ListClearedEvent;

        /// <summary>
        /// Invoked right before the list will be cleared. All the elements are still present in the list at this state.
        /// </summary>
        public event EventHandler<EventArgs>? ListClearingEvent;

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (_shouldFireEvents)
            {
                ItemInsertedEvent?.Invoke(this, new ObservableListInsertEventArgs<T>(item, index));
            }
        }

        protected override void RemoveItem(int index)
        {
            T item = this[index];
            base.RemoveItem(index);

            if (_shouldFireEvents)
            {
                ItemRemovedEvent?.Invoke(this, new ObservableListRemoveEventArgs<T>(item, index));
            }
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];
            base.SetItem(index, item);

            if (_shouldFireEvents)
            {
                ItemRemovedEvent?.Invoke(this, new ObservableListRemoveEventArgs<T>(oldItem, index));
                ItemInsertedEvent?.Invoke(this, new ObservableListInsertEventArgs<T>(item, index));
            }
        }

        protected override void ClearItems()
        {
            if (_shouldFireEvents)
            {
                ListClearingEvent?.Invoke(this, EventArgs.Empty);
            }

            base.ClearItems();

            if (_shouldFireEvents)
            {
                ListClearedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Move(T item, int newIndex)
        {
            _shouldFireEvents = false;

            int idx = IndexOf(item);
            if (idx == newIndex)
            {
                return true;
            }

            if (idx == -1)
            {
                return false;
            }

            RemoveAt(idx);
            Insert(newIndex, item);
            _shouldFireEvents = true;

            ItemMovedEvent?.Invoke(this, new ObservableListMoveEventArgs<T>(item, idx, newIndex));
            return true;
        }

        public void AddRange(IEnumerable<T> items)
        {
            IEnumerator<T> enumerator = items.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Add(enumerator.Current);
            }

            enumerator.Dispose();
        }

        public void AddItems(params T[] items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }
    }

    public class ObservableListInsertEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }
        public int Index { get; private set; }

        public ObservableListInsertEventArgs(T item, int index)
        {
            Item = item;
            Index = index;
        }
    }

    public class ObservableListRemoveEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }
        public int Index { get; private set; }

        public ObservableListRemoveEventArgs(T item, int index)
        {
            Item = item;
            Index = index;
        }
    }

    public class ObservableListMoveEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }

        public ObservableListMoveEventArgs(T item, int oldIndex, int newIndex)
        {
            Item = item;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }
}
