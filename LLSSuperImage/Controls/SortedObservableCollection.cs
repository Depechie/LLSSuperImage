using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LLSSuperImage.Controls
{
    public class SortedObservableCollection<T> : ObservableCollection<T> where T : IComparable<T>
    {
        protected override void InsertItem(int index, T item)
        {
            index = Array.BinarySearch<T>(this.Items.ToArray<T>(), item, null);
            if (index >= 0)
            {
                //throw new ArgumentException("Cannot insert duplicated items");
            }
            else
                base.InsertItem(~index, item);
        }
    }
}
