namespace Client.MultiSelector
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public class MultiSelectorBehavior
    {
        #region Fields

        public static readonly DependencyProperty SynchronizedSelectedItems = DependencyProperty.RegisterAttached(
            "SynchronizedSelectedItems",
            typeof(IList),
            typeof(MultiSelectorBehavior),
            new PropertyMetadata(null, OnSynchronizedSelectedItemsChanged));

        private static readonly DependencyProperty _synchronizationManagerProperty = DependencyProperty.RegisterAttached(
            "_synchronizationManager",
            typeof(SynchronizationManager),
            typeof(MultiSelectorBehavior),
            new PropertyMetadata(null));

        #endregion

        #region Methods

        public static IList GetSynchronizedSelectedItems(DependencyObject dependencyObject)
        {
            return (IList)dependencyObject.GetValue(SynchronizedSelectedItems);
        }

        public static void SetSynchronizedSelectedItems(DependencyObject dependencyObject, IList value)
        {
            dependencyObject.SetValue(SynchronizedSelectedItems, value);
        }

        private static SynchronizationManager Get_synchronizationManager(DependencyObject dependencyObject)
        {
            return (SynchronizationManager)dependencyObject.GetValue(_synchronizationManagerProperty);
        }

        private static void Set_synchronizationManager(DependencyObject dependencyObject, SynchronizationManager value)
        {
            dependencyObject.SetValue(_synchronizationManagerProperty, value);
        }

        private static void OnSynchronizedSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                SynchronizationManager synchronizer = Get_synchronizationManager(dependencyObject);
                synchronizer.StopSynchronizing();
                Set_synchronizationManager(dependencyObject, null);
            }

            if (!(e.NewValue is IList) || !(dependencyObject is Selector selector))
            {
                return;
            }

            {
                SynchronizationManager synchronizer = Get_synchronizationManager(dependencyObject);

                if (synchronizer == null)
                {
                    synchronizer = new SynchronizationManager(selector);
                    Set_synchronizationManager(dependencyObject, synchronizer);
                }

                synchronizer.StartSynchronizingList();
            }
        }

        #endregion

        #region Classes

        private class SynchronizationManager
        {
            #region Fields

            private readonly Selector _multiSelector;
            private TwoListSynchronizer _synchronizer;

            #endregion

            #region Constructors

            internal SynchronizationManager(Selector selector)
            {
                _multiSelector = selector;
            }

            #endregion

            #region Methods

            public void StartSynchronizingList()
            {
                IList list = GetSynchronizedSelectedItems(_multiSelector);

                if (list == null)
                {
                    return;
                }

                _synchronizer = new TwoListSynchronizer(GetSelectedItemsCollection(_multiSelector), list);
                _synchronizer.StartSynchronizing();
            }

            public void StopSynchronizing()
            {
                _synchronizer.StopSynchronizing();
            }

            private static IList GetSelectedItemsCollection(Selector selector)
            {
                switch (selector)
                {
                    case MultiSelector multiSelector:
                        return multiSelector.SelectedItems;
                    case ListBox box:
                        return box.SelectedItems;
                    default:
                        throw new InvalidOperationException("Target object has no SelectedItems property to bind.");
                }
            }

            #endregion
        }

        #endregion
    }
}
