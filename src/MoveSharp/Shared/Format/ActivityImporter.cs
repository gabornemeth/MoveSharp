using MoveSharp.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MoveSharp.Format
{
    /// <summary>
    /// Load and save activity to a file format
    /// </summary>
    public abstract class ActivityImporter
    {
        private MemoryActivity _activity;

        public MemoryActivity Activity
        {
            get
            {
                return _activity;
            }
        }

        public ActivityImporter(MemoryActivity activity)
        {
            if (activity == null)
                throw new ArgumentException("activity");
            _activity = activity;
        }

        public abstract void Load(Stream source);

        public async Task LoadAsync(Stream source)
        {
            await Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Load(source);
                sw.Stop();
                Debug.WriteLine("Loading activity {0}: {1} sec", _activity.Name, sw.ElapsedMilliseconds / 1000.0f);
            }).ConfigureAwait(false);
        }
    }
}
