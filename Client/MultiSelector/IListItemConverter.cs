namespace Client.MultiSelector
{
    public interface IListItemConverter
    {
        #region Methods

        object Convert(object masterListItem);

        object ConvertBack(object targetListItem);

        #endregion
    }
}
