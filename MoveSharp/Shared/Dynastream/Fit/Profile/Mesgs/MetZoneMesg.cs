#region Copyright
////////////////////////////////////////////////////////////////////////////////
// The following FIT Protocol software provided may be used with FIT protocol
// devices only and remains the copyrighted property of Dynastream Innovations Inc.
// The software is being provided on an "as-is" basis and as an accommodation,
// and therefore all warranties, representations, or guarantees of any kind
// (whether express, implied or statutory) including, without limitation,
// warranties of merchantability, non-infringement, or fitness for a particular
// purpose, are specifically disclaimed.
//
// Copyright 2017 Dynastream Innovations Inc.
////////////////////////////////////////////////////////////////////////////////
// ****WARNING****  This file is auto-generated!  Do NOT edit this file.
// Profile Version = 20.43Release
// Tag = production/akw/20.43.00-0-gc29a67f
////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;

namespace Dynastream.Fit
{
    /// <summary>
    /// Implements the MetZone profile message.
    /// </summary>
    public class MetZoneMesg : Mesg
    {
        #region Fields
        #endregion

        /// <summary>
        /// Field Numbers for <see cref="MetZoneMesg"/>
        /// </summary>
        public sealed class FieldDefNum
        {
            public const byte MessageIndex = 254;
            public const byte HighBpm = 1;
            public const byte Calories = 2;
            public const byte FatCalories = 3;
            public const byte Invalid = Fit.FieldNumInvalid;
        }

        #region Constructors
        public MetZoneMesg() : base(Profile.GetMesg(MesgNum.MetZone))
        {
        }

        public MetZoneMesg(Mesg mesg) : base(mesg)
        {
        }
        #endregion // Constructors

        #region Methods
        ///<summary>
        /// Retrieves the MessageIndex field</summary>
        /// <returns>Returns nullable ushort representing the MessageIndex field</returns>
        public ushort? GetMessageIndex()
        {
            Object val = GetFieldValue(254, 0, Fit.SubfieldIndexMainField);
            if(val == null)
            {
                return null;
            }

            return (Convert.ToUInt16(val));
            
        }

        /// <summary>
        /// Set MessageIndex field</summary>
        /// <param name="messageIndex_">Nullable field value to be set</param>
        public void SetMessageIndex(ushort? messageIndex_)
        {
            SetFieldValue(254, 0, messageIndex_, Fit.SubfieldIndexMainField);
        }
        
        ///<summary>
        /// Retrieves the HighBpm field</summary>
        /// <returns>Returns nullable byte representing the HighBpm field</returns>
        public byte? GetHighBpm()
        {
            Object val = GetFieldValue(1, 0, Fit.SubfieldIndexMainField);
            if(val == null)
            {
                return null;
            }

            return (Convert.ToByte(val));
            
        }

        /// <summary>
        /// Set HighBpm field</summary>
        /// <param name="highBpm_">Nullable field value to be set</param>
        public void SetHighBpm(byte? highBpm_)
        {
            SetFieldValue(1, 0, highBpm_, Fit.SubfieldIndexMainField);
        }
        
        ///<summary>
        /// Retrieves the Calories field
        /// Units: kcal / min</summary>
        /// <returns>Returns nullable float representing the Calories field</returns>
        public float? GetCalories()
        {
            Object val = GetFieldValue(2, 0, Fit.SubfieldIndexMainField);
            if(val == null)
            {
                return null;
            }

            return (Convert.ToSingle(val));
            
        }

        /// <summary>
        /// Set Calories field
        /// Units: kcal / min</summary>
        /// <param name="calories_">Nullable field value to be set</param>
        public void SetCalories(float? calories_)
        {
            SetFieldValue(2, 0, calories_, Fit.SubfieldIndexMainField);
        }
        
        ///<summary>
        /// Retrieves the FatCalories field
        /// Units: kcal / min</summary>
        /// <returns>Returns nullable float representing the FatCalories field</returns>
        public float? GetFatCalories()
        {
            Object val = GetFieldValue(3, 0, Fit.SubfieldIndexMainField);
            if(val == null)
            {
                return null;
            }

            return (Convert.ToSingle(val));
            
        }

        /// <summary>
        /// Set FatCalories field
        /// Units: kcal / min</summary>
        /// <param name="fatCalories_">Nullable field value to be set</param>
        public void SetFatCalories(float? fatCalories_)
        {
            SetFieldValue(3, 0, fatCalories_, Fit.SubfieldIndexMainField);
        }
        
        #endregion // Methods
    } // Class
} // namespace
