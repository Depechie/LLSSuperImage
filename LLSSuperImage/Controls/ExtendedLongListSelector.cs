using System.Collections.Specialized;
using System.Windows;
using Microsoft.Phone.Controls;

namespace LLSSuperImage.Controls
{
    public class ExtendedLongListSelector : LongListSelector
    {
        public ExtendedLongListSelector()
        {
            Loaded += (sender, args) =>
            {
                ((INotifyCollectionChanged)ItemsSource).CollectionChanged += (sender2, args2) =>
                {
                    if (ItemsSource.Count > 0 && args2.NewItems != null)
                        Deployment.Current.Dispatcher.BeginInvoke(() => this.ScrollTo(ItemsSource[0]));
                };
            };
        }
    }
}
