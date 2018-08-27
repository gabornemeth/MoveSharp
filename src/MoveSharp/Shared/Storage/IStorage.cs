using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoveSharp.Storage
{
    /// <summary>
    /// Different I/O based methods, needed by the application
    /// </summary>
    public interface IStorage
    {
        Task<ILocalFolder> GetFolderAsync(string name);
        Task<ILocalFolder> CreateFolderAsync(string name);
        Task<ILocalFile> GetFileAsync(string path);
        /// <summary>
        /// Path of the root folder
        /// </summary>
        string RootFolderPath { get; }
    }

    public interface IStorageItem
    {
        /// <summary>
        /// Name of the item (not full path)
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Full path of the file
        /// </summary>
        string Path { get; }

        IStorage Storage { get; }
    }

    public enum CreateFileOption
    {
        FailIfExists,
        ReplaceExisting
    }

    public interface ILocalFolder : IStorageItem
    {
        //Task<ILocalFolder> GetFolderAsync(string name);
        Task<ILocalFile> GetFileAsync(string name);
        Task<ILocalFile> CreateFileAsync(string name);
        Task<ILocalFile> CreateFileAsync(string name, CreateFileOption options);
        Task<List<ILocalFile>> GetFilesAsync();
        Task<ILocalFolder> CreateFolderAsync(string name);
    }
}
