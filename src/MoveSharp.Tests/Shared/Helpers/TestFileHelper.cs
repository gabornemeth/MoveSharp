using System.IO;
using System.Threading.Tasks;
using MoveSharp.Storage;

namespace MoveSharp.Tests.Helpers
{
    /// <summary>
    /// File helper
    /// </summary>
    class TestFileHelper
    {
        public static IStorage Storage { get; }

        static TestFileHelper()
        {
            var assemblyFolder = Path.GetDirectoryName(typeof(TestFileHelper).Assembly.Location);
            Storage = new LocalStorage(assemblyFolder);
        }

        public static async Task<Stream> OpenForReadAsync(string path)
        {
            var file = await GetFileAsync(path, false);
            if (file == null)
                return null;
            var stream = await file.OpenForReadAsync();

            return stream;
        }

        public static async Task<Stream> OpenForWriteAsync(string path)
        {
            var file = await GetFileAsync(path, true);
            if (file == null)
                return null;
            var stream = await file.OpenForWriteAsync();

            return stream;
        }

        private static async Task<ILocalFile> GetFileAsync(string path, bool create)
        {
            var folderName = Path.GetDirectoryName(path);
            var folder = await Storage.GetFolderAsync(folderName);
            if (folder == null)
                return null;

            var file = await folder.GetFileAsync(Path.GetFileName(path));
            if (file == null)
                file = await folder.CreateFileAsync(Path.GetFileName(path));
            return file;
        }
    }
}
