//
// LocalFile.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.IO;
using System.Threading.Tasks;

namespace MoveSharp.Storage
{
    /// <summary>
    /// Local file in Android
    /// </summary>
    class LocalFile : LocalStorageItem, ILocalFile
    {
        private readonly FileInfo _file;

        public LocalFile(IStorage storage, FileInfo file) : base(storage)
        {
            this._file = file;
        }

        public string Name
        {
            get { return _file.Name; }
        }

        public int Size
        {
            get
            {
                return (int)_file.Length;
            }
        }

        public async Task<Stream> OpenForWriteAsync()
        {
            return await Task.Run<Stream>(() => { return _file.Open(FileMode.Create, FileAccess.ReadWrite); });
        }

        public async Task<Stream> OpenForReadAsync()
        {
            return await Task.Run<Stream>(() => { return _file.OpenRead(); });
        }

        public async Task RenameAsync(string destName)
        {
            await CopyAsync(System.IO.Path.Combine(_file.DirectoryName, destName));
            await DeleteAsync();
        }

        public Task DeleteAsync()
        {
            var deleteTask = Task.Run(() => { _file.Delete(); });
            deleteTask.ConfigureAwait(false);
            return deleteTask;
        }

        public Task CopyAsync(string pathDest)
        {
            var copyTask = Task.Run(() => File.Copy(_file.FullName, pathDest));
            copyTask.ConfigureAwait(false);
            return copyTask;
        }

        public Task GetPropertiesAsync()
        {
            return Task.Run(() => { });
        }

        public Task<ILocalFolder> GetParentFolderAsync()
        {
            return Task.FromResult(new LocalFolder(Storage, _file.Directory) as ILocalFolder);
        }

        public string Path
        {
            get { return _file.FullName; }
        }
    }
}
