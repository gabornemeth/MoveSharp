//
// PPTTrackerViewModel.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using System;
using MoveSharp.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MoveSharp.ViewModel;

namespace MoveSharp.PolarPersonalTrainer
{
    /// <summary>
    /// Viewmodel for PolarPersonalTrainer.com tracker
    /// </summary>
    public class PPTTrackerViewModel : TrackerViewModel
    {
        private PPTTracker _tracker;

        public PPTTrackerViewModel(ViewModelLocator viewModelLocator, PPTTracker tracker, TrackerViewModelCollection trackers)
            : base(viewModelLocator, tracker, trackers)
        {
            _tracker = tracker;
        }

        private DateTime _lastTime = DateTime.MinValue;

        protected override async Task<IEnumerable<IActivitySummary>> GetActivitiesAsync()
        {
            if (ActivitiesInternal.Count == 0)
            {
                if (_lastTime == DateTime.MinValue)
                    _lastTime = DateTime.Now;
                var activities = await Model.GetActivitiesAsync(_lastTime);
                if (activities.Count() == 0)
                    _lastTime = _lastTime.AddMonths(-1);
                return activities;
            }
            else
            {
                var lastActivity = ActivitiesInternal.Last();
                return await Model.GetActivitiesAsync(lastActivity.Model.StartTime);
            }
        }
    }
}
