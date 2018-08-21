using System;
using System.Collections.Generic;
using System.Linq;

namespace MoveSharp.Models
{
    public class LapSummaryCollection : IEnumerable<ILapSummary>
    {
        private List<ILapSummary> _items;
        private ILapSummary _current;

        /// <summary>
        /// Current lap (not finished)
        /// </summary>
        public ILapSummary Current
        {
            get { return _current; }
        }

        public LapSummaryCollection()
        {
            _items = new List<ILapSummary>();
        }

        public void Clear()
        {
            _items.Clear();
            _current = null;
        }

        //public void ChangeLast(ILapSummary newLastLap)
        //{
        //    if (_items.Count > 0)
        //        _items.RemoveAt(_items.Count - 1);
        //    _items.Add(newLastLap);
        //}

        /// <summary>
        /// Starts a new lap
        /// Current is being finished and pushed to saved laps
        /// </summary>
        public void NewLap(ILapSummary lap)
        {
            if (_current != null)
                _items.Add(_current);

            _current = lap;
        }

        /// <summary>
        /// Adds new lap to the collection
        /// </summary>
        /// <param name="lap"></param>
        public void Add(ILapSummary lap)
        {
            //if (_current != null)
            //    _items.Add(_current);
            _items.Add(lap);

            //_current = ;

            //var idxInsertBefore = -1;
            //for (int i = 0; i < _items.Count; i++)
            //{
            //    if (_items[i].StartTime > lap.StartTime)
            //    {
            //        idxInsertBefore = i;
            //        break;
            //    }
            //}

            //if (idxInsertBefore == -1)
            //    _items.Add(lap);
            //else
            //    _items.Insert(idxInsertBefore, lap);
        }

        public IEnumerator<ILapSummary> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
