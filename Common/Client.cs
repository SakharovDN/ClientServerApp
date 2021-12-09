namespace Common
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        #region Properties

        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        #endregion
    }
}
