//
// FileHelper.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//
using System.Threading.Tasks;
using MoveSharp.Storage;
using System.IO;

namespace MoveSharp.Helpers
{
    public class FileHelper
    {
        public const string FitExtension = ".fit";
        public const string GpxExtension = ".gpx";
        public const string TcxExtension = ".tcx";
        public const string HrmExtension = ".hrm";

        /// <summary>
        /// Gets file size as string
        /// </summary>
        /// <param name="size">size as integer</param>
        /// <returns></returns>
        public static string GetSizeString(long size)
        {
            var units = new[] { "bytes", "KB", "MB", "GB" };
            int i = 0;
            double dSize = size;
            while (dSize > 1024)
            {
                dSize /= 1024;
                i++;
            }
            return string.Format("{0} {1}", dSize.ToString("F0"), units[i]);
        }

        private const string TempFolderName = "Temp";

        public static async Task<ILocalFile> GetTempFileAsync(IStorage storage, string fileName)
        {
            var tempFolder = await storage.GetFolderAsync(TempFolderName);
            if (tempFolder == null)
                tempFolder = await storage.CreateFolderAsync(TempFolderName);

            return await tempFolder.CreateFileAsync(fileName, CreateFileOption.ReplaceExisting);
        }
    }
}
