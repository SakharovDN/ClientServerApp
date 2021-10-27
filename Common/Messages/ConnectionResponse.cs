namespace Common.Messages
{
    using _Enums_;

    public class ConnectionResponse
    {
        #region Properties

        public ResultCodes Result { get; set; }

        public string Reason { get; set; }

        #endregion
    }
}
