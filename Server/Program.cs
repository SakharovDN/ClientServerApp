namespace Server
{
    using System;
    using Common;

    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            try
            {
                var networkManager = new NetworkManager();
                networkManager.Start();

                Console.ReadLine();

                networkManager.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        #endregion
    }
}
