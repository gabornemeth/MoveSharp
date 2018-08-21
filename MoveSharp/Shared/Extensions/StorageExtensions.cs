//
// StorageExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Threading.Tasks;
using System.Linq;
using XTools;
using MoveSharp.Storage;
using System.IO;

namespace MoveSharp.Extensions
{
    public static class StorageExtensions
    {
        public static string GetNameWithoutExtension(this ILocalFile file)
        {
            int indexOfDot = file.Name.LastIndexOf('.');
            if (indexOfDot == -1)
                return file.Name;
            return file.Name.Substring(0, indexOfDot);
        }

        public static async Task<ILocalFile> TryGetItemAsync(this ILocalFolder folder, string name)
        {
            var files = await folder.GetFilesAsync().ConfigureAwait(false);
            return files.FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Opening existing storage file, or creating new if does not exist
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async static Task<ILocalFile> OpenOrCreateAsync(this ILocalFolder folder, string name)
        {
            var exists = await folder.TryGetItemAsync(name); // check whether the file exists
            if (exists != null)
                return await folder.GetFileAsync(name);

            return await folder.CreateFileAsync(name);
        }

        /// <summary>
        /// Gets unique file name in the folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async static Task<string> GetUniqueFileNameAsync(this ILocalFolder folder, string name = null)
        {
            if (name == null)
                name = XTools.StringExtensions.GetRandomString(20);

            int numberToAppend = 0;
            var file = await folder.TryGetItemAsync(name);
            if (file == null)
                return name;

            var nameUnique = name;
            do
            {
                numberToAppend++;
                nameUnique = string.Format("{0} ({1}){2}", Path.GetFileNameWithoutExtension(name), numberToAppend, Path.GetExtension(name));
            }
            while (await folder.TryGetItemAsync(nameUnique) != null); // as long as nameUnique named file exists

            return nameUnique;
        }

        public static async Task<ILocalFolder> GetOrCreateFolderAsync(this IStorage storage, string name)
        {
            var folder = await storage.GetFolderAsync(name);
            if (folder == null)
                folder = await storage.CreateFolderAsync(name);

            return folder;
        }

    }
}
