namespace Client.MultiSelector
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;

    public class TwoListSynchronizer : IWeakEventListener
    {
        #region Fields

        private static readonly IListItemConverter _defaultConverter = new DoNothingListItemConverter();
        private readonly IList _masterList;
        private readonly IListItemConverter _masterTargetConverter;
        private readonly IList _targetList;

        #endregion

        #region Delegates

        private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);

        #endregion

        #region Constructors

        public TwoListSynchronizer(IList masterList, IList targetList, IListItemConverter masterTargetConverter)
        {
            _masterList = masterList;
            _targetList = targetList;
            _masterTargetConverter = masterTargetConverter;
        }

        public TwoListSynchronizer(IList masterList, IList targetList)
            : this(masterList, targetList, _defaultConverter)
        {
        }

        #endregion

        #region Methods

        public void StartSynchronizing()
        {
            ListenForChangeEvents(_masterList);
            ListenForChangeEvents(_targetList);
            SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);

            if (!TargetAndMasterCollectionsAreEqual())
            {
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
            }
        }

        public void StopSynchronizing()
        {
            StopListeningForChangeEvents(_masterList);
            StopListeningForChangeEvents(_targetList);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);
            return true;
        }

        protected void ListenForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
            }
        }

        protected void StopListeningForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged changed)
            {
                CollectionChangedEventManager.RemoveListener(changed, this);
            }
        }

        private static void AddItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int itemCount = e.NewItems.Count;

            for (int i = 0; i < itemCount; i++)
            {
                int insertionPoint = e.NewStartingIndex + i;

                if (insertionPoint > list.Count)
                {
                    list.Add(converter(e.NewItems[i]));
                }
                else
                {
                    list.Insert(insertionPoint, converter(e.NewItems[i]));
                }
            }
        }

        private object ConvertFromMasterToTarget(object masterListItem)
        {
            return _masterTargetConverter == null ? masterListItem : _masterTargetConverter.Convert(masterListItem);
        }

        private object ConvertFromTargetToMaster(object targetListItem)
        {
            return _masterTargetConverter == null ? targetListItem : _masterTargetConverter.ConvertBack(targetListItem);
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var sourceList = sender as IList;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    PerformActionOnAllLists(AddItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    PerformActionOnAllLists(MoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    PerformActionOnAllLists(RemoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    PerformActionOnAllLists(ReplaceItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateListsFromSource(sender as IList);
                    break;
            }
        }

        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void PerformActionOnAllLists(ChangeListAction action, IEnumerable sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            if (Equals(sourceList, _masterList))
            {
                PerformActionOnList(_targetList, action, collectionChangedArgs, ConvertFromMasterToTarget);
            }
            else
            {
                PerformActionOnList(_masterList, action, collectionChangedArgs, ConvertFromTargetToMaster);
            }
        }

        private void PerformActionOnList(
            IList list,
            ChangeListAction action,
            NotifyCollectionChangedEventArgs collectionChangedArgs,
            Converter<object, object> converter)
        {
            StopListeningForChangeEvents(list);
            action(list, collectionChangedArgs, converter);
            ListenForChangeEvents(list);
        }

        private static void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int itemCount = e.OldItems.Count;

            for (int i = 0; i < itemCount; i++)
            {
                list.RemoveAt(e.OldStartingIndex);
            }
        }

        private static void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void SetListValuesFromSource(IEnumerable sourceList, IList targetList, Converter<object, object> converter)
        {
            StopListeningForChangeEvents(targetList);
            targetList.Clear();

            foreach (object o in sourceList)
            {
                targetList.Add(converter(o));
            }

            ListenForChangeEvents(targetList);
        }

        private bool TargetAndMasterCollectionsAreEqual()
        {
            return _masterList.Cast<object>().SequenceEqual(_targetList.Cast<object>().Select(ConvertFromTargetToMaster));
        }

        private void UpdateListsFromSource(IEnumerable sourceList)
        {
            if (Equals(sourceList, _masterList))
            {
                SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);
            }
            else
            {
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
            }
        }

        #endregion

        #region Classes

        internal class DoNothingListItemConverter : IListItemConverter
        {
            #region Methods

            public object Convert(object masterListItem)
            {
                return masterListItem;
            }

            public object ConvertBack(object targetListItem)
            {
                return targetListItem;
            }

            #endregion
        }

        #endregion
    }
}
