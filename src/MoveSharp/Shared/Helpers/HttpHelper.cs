using System;
using System.Threading.Tasks;
using XTools.Diagnostics;

namespace MoveSharp
{
    public class HttpHelper
    {
        private const int MaxNumberOfRetries = 0;

        public static async Task Execute(Func<Task> action)
        {
            // there is NameResolutionException or other errors sometimes with HttpClient
            // retry it a couple of times, before state as unsuccessful
            bool hasError = false;
            var retryCount = 0;
            do
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    hasError = true;
                    retryCount++;
                    Log.Error(ex);
                }
            } while (hasError && retryCount < MaxNumberOfRetries);
        }
    }
}
   