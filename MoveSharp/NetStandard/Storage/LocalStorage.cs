//
// LocalStorage.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace MoveSharp.Storage
{
    /// <summary>
    /// Android file handler component
    /// </summary>
    public class LocalStorage : IStorage
    {
        public LocalStorage(string rootFolderPath)
        {
            RootFolderPath = rootFolderPath;
        }
        private string GetFolderPath(string folder)
        {
            return Path.Combine(RootFolderPath, folder);
        }

        public async Task<ILocalFolder> GetFolderAsync(string name)
        {
            return await Task.Run<ILocalFolder>(() =>
                {
                    var path = GetFolderPath(name);
                    if (Directory.Exists(path))
                        return new LocalFolder(this, new DirectoryInfo(path));

                    return null;
                });
        }

        public async Task<ILocalFolder> CreateFolderAsync(string name)
        {
            return await Task.Run<ILocalFolder>(() =>
            {
                var path = GetFolderPath(name); // full path of the desired folder
                DirectoryInfo dirInfo = null;
                if (Directory.Exists(path))
                    dirInfo = new DirectoryInfo(path);
                else
                    dirInfo = Directory.CreateDirectory(path);
                return new LocalFolder(this, dirInfo);
            });
        }

        public async Task<ILocalFile> GetFileAsync(string path)
        {
            return await Task.Run<ILocalFile>(() =>
                {
                    var fileInfo = new FileInfo(path);
                    if (!fileInfo.Exists)
                        return null;

                    return new LocalFile(this, fileInfo);
                });
        }

        public string RootFolderPath
        {
            get; private set;
        }
    }
}
