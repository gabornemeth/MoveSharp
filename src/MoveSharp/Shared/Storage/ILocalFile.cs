
using System.IO;
using System.Threading.Tasks;
namespace MoveSharp.Storage
{
    /// <summary>
    /// Interface for a file in local filesystem (on the phone)
    /// </summary>
    public interface ILocalFile : IStorageItem
    {
        /// <summary>
        /// Size in bytes
        /// </summary>
        int Size { get; }

        Task GetPropertiesAsync();

        Task<Stream> OpenForWriteAsync();
        Task<Stream> OpenForReadAsync();
        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="destName">the file name we want rename to</param>
        /// <returns></returns>
        Task RenameAsync(string destName);
        /// <summary>
        /// Delete the file
        /// </summary>
        Task DeleteAsync();

        /// <summary>
        /// Copies the file
        /// </summary>
        /// <param name="destName"></param>
        /// <returns></returns>
        Task CopyAsync(string destName);

        Task<ILocalFolder> GetParentFolderAsync();
    }
}