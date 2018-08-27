//
// FitStripper.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using Dynastream.Fit;
using System;
using System.Collections.Generic;
using System.IO;

namespace MoveSharp.Strippers
{
    /// <summary>
    /// FIT file stripper
    /// </summary>
    public class FitStripper : IStripper
    {
        private readonly Dictionary<ushort, int> _mesgCounts = new Dictionary<ushort, int>();
        //private Stream _fitSource, _fitDest;
        private Encode _encoder;
        private StripOptions _options;

        public bool LogEnabled { get; set; }

        public FitStripper()
        {
            LogEnabled = false;
        }

        private void Log(string format, params object[] args)
        {
            if (LogEnabled)
            {
                XTools.Diagnostics.Log.Write(string.Format(format, args));
            }
        }

        public void Strip(Stream input, Stream output, StripOptions options)
        {
            int time = Environment.TickCount;

            _options = options;

            // Create file encode object
            _encoder = new Encode(output);

            Decode decoder = new Decode();
            //MesgBroadcaster mesgBroadcaster = new MesgBroadcaster();

            // Connect the Broadcaster to our event (message) source (in this case the Decoder)
            decoder.MesgEvent += OnMesg;
            decoder.MesgDefinitionEvent += OnMesgDefn;

            // Subscribe to message events of interest by connecting to the Broadcaster
            //mesgBroadcaster.MesgEvent += OnMesg;
            //mesgBroadcaster.MesgDefinitionEvent += OnMesgDefn;

            _mesgCounts.Clear();

            bool status = decoder.IsFIT(input);
            status &= decoder.CheckIntegrity(input);
            // Process the file
            if (status)
            {
                Log("Decoding...");
                decoder.Read(input);
                Log("Decoded FIT file");
            }
            else
            {
                try
                {
                    Log("Integrity Check Failed.");
                    Log("Attempting to decode...");
                    decoder.Read(input);
                }
                catch (FitException ex)
                {
                    Log("DecodeDemo caught FitException: " + ex.Message);
                }
            }

            _encoder.Close();

            Log("Summary:");
            int totalMesgs = 0;
            foreach (KeyValuePair<ushort, int> pair in _mesgCounts)
            {
                Log("MesgID {0,3} Count {1}", pair.Key, pair.Value);
                totalMesgs += pair.Value;
            }

            Log("{0} Message Types {1} Total Messages", _mesgCounts.Count, totalMesgs);

            time = Environment.TickCount - time;
            Log("Time elapsed: {0:0.#}s", time / 1000.0f);// stopwatch.Elapsed.TotalSeconds);
        }

        #region Message Handlers

        // Client implements their handlers of interest and subscribes to MesgBroadcaster events
        void OnMesgDefn(object sender, MesgDefinitionEventArgs e)
        {
            Log("OnMesgDef: Received Defn for local message #{0}, global num {1}", e.mesgDef.LocalMesgNum, e.mesgDef.GlobalMesgNum);
            Log("\tIt has {0} fields and is {1} bytes long", e.mesgDef.NumFields, e.mesgDef.GetMesgSize());
            _encoder.Write(e.mesgDef);
        }

        void OnMesg(object sender, MesgEventArgs e)
        {
            Log("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name);

            // remove not wanted fields
            for (int i = e.mesg.GetNumFields() - 1; i >= 0; i--)
            {
                var field = e.mesg.FieldsList[i];
                if (((_options & StripOptions.HeartRate) != StripOptions.None && field.Name.EndsWith("HeartRate")) ||
                    ((_options & StripOptions.Power) != StripOptions.None && field.Name.EndsWith("Power")) ||
                    ((_options & StripOptions.Cadence) != StripOptions.None && field.Name.EndsWith("Cadence")))
                {
                    e.mesg.FieldsList.RemoveAt(i);
                }
            }

            for (var i = 0; i < e.mesg.GetNumFields(); i++)
            {
                for (var j = 0; j < e.mesg.FieldsList[i].GetNumValues(); j++)
                {
                    var field = e.mesg.FieldsList[i];
                    Log("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3} (raw value {5})", i, j,
                        field.GetName(), field.GetValue(j), field.Num, field.GetRawValue(j));
                }
            }

            if (_mesgCounts.ContainsKey(e.mesg.Num))
            {
                _mesgCounts[e.mesg.Num]++;
            }
            else
            {
                _mesgCounts.Add(e.mesg.Num, 1);
            }
            _encoder.Write(e.mesg);
        }

        #endregion
    }
}
