using XTools.Diagnostics;
using MoveSharp.Extensions;
using MoveSharp.Format;
using MoveSharp.Storage;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    /// <summary>
    /// Activity in FIT file
    /// </summary>
    public class LocalActivitySummary : ActivitySummary
    {
        /// <summary>
        /// Name of the activities folder
        /// </summary>
        public const string FolderName = "Activities";

        private readonly ILocalFile _file;

        [JsonIgnore]
        public ILocalFile File
        {
            get { return _file; }
        }

        public LocalActivitySummary(ILocalFile file)
        {
            this._file = file;
        }

        [JsonIgnore]
        public string MetaDataFileName
        {
            get
            {
                return _file.Name + ".metadata";
            }
        }

        private async Task<ILocalFolder> GetFolderAsync()
        {
            var folder = await _file.GetParentFolderAsync();
            return folder;
        }

        public override async Task GetPropertiesAsync()
        {
            try
            {
                var folder = await GetFolderAsync();
                Name = Path.GetFileNameWithoutExtension(_file.Name);
                var metaDataFile = await folder.GetFileAsync(MetaDataFileName);
                bool decodedFromMetaData = false;
                if (metaDataFile != null)
                {
                    try
                    {
                        // read properties from JSON metadata file - way faster than decoding FIT file
                        using (var stream = await metaDataFile.OpenForReadAsync())
                        {
                            using (var textReader = new StreamReader(stream))
                            {
                                using (var jsonReader = new JsonTextReader(textReader))
                                {
                                    var serializer = new JsonSerializer();
                                    var summary = serializer.Deserialize<ActivitySummary>(jsonReader);
                                    CopyFrom(summary);
                                    // workaround (v1.8): start time has not been serialized in the first versions
                                    if (StartTime != System.DateTime.MinValue)
                                        decodedFromMetaData = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                if (!decodedFromMetaData)
                {
                    await _file.GetPropertiesAsync(); // get file properties
                    var time = Environment.TickCount;
                    // decode activity and refresh summary info
                    var activityTmp = new MemoryActivity() { Name = this.Name };
                    await DecodeAsync(activityTmp);
                    CopyFrom(activityTmp);

                    Log.Diagnostics("Decoding of {0} took {1} sec", _file.Name, (Environment.TickCount - time) / 1000.0f);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public override void CopyFrom(IActivitySummary source)
        {
            if (source == null)
                return;

            base.CopyFrom(source);
            StartTime = source.StartTime;
        }

        /// <summary>
        /// write properties into JSON metadata file
        /// </summary>
        /// <returns></returns>
        private async Task UpdateMetadata(ActivitySummary summary)
        {
            var folder = await GetFolderAsync();
            var metaDataFile = await folder.CreateFileAsync(MetaDataFileName, CreateFileOption.ReplaceExisting);
            using (var stream = await metaDataFile.OpenForWriteAsync())
            {
                using (var textWriter = new StreamWriter(stream))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(textWriter, summary);
                }
            }
        }

        public async Task DecodeAsync(MemoryActivity dest = null)
        {
            Name = _file.GetNameWithoutExtension();
            var input = await _file.OpenForReadAsync(); // file stream
            if (input.Length == 0)
                return;
            MemoryActivity activity = dest != null ? dest : new MemoryActivity();
            // use the appropriate importer type
            ActivityImporter importer = null;
            if (_file.Name.EndsWith(".fit", StringComparison.OrdinalIgnoreCase))
                importer = new FitImporter(activity);
            else if (_file.Name.EndsWith(".tcx", StringComparison.OrdinalIgnoreCase))
                importer = new TcxImporter(activity);
            if (importer != null)
                await importer.LoadAsync(input);

            await UpdateMetadata(ActivitySummary.FromActivity(dest));
        }

        public static bool IsSupportedType(string extension)
        {
            return extension.Equals(".fit", StringComparison.OrdinalIgnoreCase) || extension.Equals(".tcx", StringComparison.OrdinalIgnoreCase);
        }
    }
}
