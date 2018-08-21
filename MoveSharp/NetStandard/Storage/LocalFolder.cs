//
// LocalFolder.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MoveSharp.Extensions;

namespace MoveSharp.Storage
{
    /// <summary>
    /// Folder in the Android file system
    /// </summary>
    internal class LocalFolder : LocalStorageItem, ILocalFolder
    {
        private DirectoryInfo _folder;

        public LocalFolder(IStorage storage, DirectoryInfo folder) : base(storage)
        {
            _folder = folder;
        }

        public async Task<ILocalFile> GetFileAsync(string name)
        {
            try
            {
                var files = await Task.Run<FileInfo[]>(() => { return _folder.GetFiles(name, SearchOption.TopDirectoryOnly); });
                if (files.Length > 0)
                    return new LocalFile(Storage, files[0]);

                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public async Task<ILocalFile> CreateFileAsync(string name)
        {
            return await CreateFileAsync(name, CreateFileOption.FailIfExists);
        }

        async public Task<ILocalFile> CreateFileAsync(string name, CreateFileOption options)
        {
            return await Task.Run<ILocalFile>(() =>
                {
                    if (!_folder.Exists)
                        _folder.Create();

                    var fi = new FileInfo(System.IO.Path.Combine(_folder.FullName, name));
                    if (fi.Exists && options == CreateFileOption.FailIfExists)
                        return null;
                    else if (options == CreateFileOption.ReplaceExisting)
                    {
                        if (fi.Exists)
                            fi.Delete();
                    }
                    return new LocalFile(Storage, fi);
                });
        }

        public async Task<List<ILocalFile>> GetFilesAsync()
        {
            return await Task.Run<List<ILocalFile>>(() =>
                {
                    var files = new List<ILocalFile>();

                    foreach (var fileInfo in _folder.GetFiles())
                        files.Add(new LocalFile(Storage, fileInfo));

                    return files;
                });
        }

        public string Name
        {
            get { return _folder.Name; }
        }

        public string Path
        {
            get { return _folder.FullName; }
        }


        public async Task<ILocalFolder> CreateFolderAsync(string name)
        {
            return await Task.Run(() =>
            {

                var folderCreated = _folder.CreateSubdirectory(name);
                return new LocalFolder(Storage, folderCreated);
            });
        }
    }
}
