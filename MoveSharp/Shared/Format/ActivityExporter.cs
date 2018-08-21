using MoveSharp.Models;
using System.IO;
using System.Threading.Tasks;

namespace MoveSharp.Format
{
    /// <summary>
    /// Saves activity to a file format
    /// </summary>
    public abstract class ActivityExporter
    {
        private MemoryActivity _activity;

        protected MemoryActivity Activity
        {
            get
            {
                return _activity;
            }
        }
        
        public ActivityExporter(MemoryActivity activity)
        {
            _activity = activity;
        }

        public abstract void Save(Stream dest);

        public async Task SaveAsync(Stream dest)
        {
            await Task.Run(() => { Save(dest); });
        }
    }
}
