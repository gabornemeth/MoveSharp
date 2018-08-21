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
    /// Implements the HrmProfile profile message.
    /// </summary>
    public class HrmProfileMesg : Mesg
    {
        #region Fields
        #endregion

        /// <summary>
        /// Field Numbers for <see cref="HrmProfileMesg"/>
        /// </summary>
        public sealed class FieldDefNum
        {
            public const byte MessageIndex = 254;
            public const byte Enabled = 0;
            public const byte HrmAntId = 1;
            public const byte LogHrv = 2;
            public const byte HrmAntIdTransType = 3;
            public const byte Invalid = Fit.FieldNumInvalid;
        }

        #region Constructors
        public HrmProfileMesg() : base(Profile.GetMesg(MesgNum.HrmProfile))
        {
        }

        public HrmProfileMesg(Mesg mesg) : base(mesg)
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
        /// Retrieves the Enabled field</summary>
        /// <returns>Returns nullable Bool enum representing the Enabled field</returns>
        public Bool? GetEnabled()
        {
            object obj = GetFieldValue(0, 0, Fit.SubfieldIndexMainField);
            Bool? value = obj == null ? (Bool?)null : (Bool)obj;
            return value;
        }

        /// <summary>
        /// Set Enabled field</summary>
        /// <param name="enabled_">Nullable field value to be set</param>
        public void SetEnabled(Bool? enabled_)
        {
            SetFieldValue(0, 0, enabled_, Fit.SubfieldIndexMainField);
        }
        
        ///<summary>
        /// Retrieves the HrmAntId field</summary>
        /// <returns>Returns nullable ushort representing the HrmAntId field</returns>
        public ushort? GetHrmAntId()
        {
            Object val = GetFieldValue(1, 0, Fit.SubfieldIndexMainField);
            if(val == null)
            {
                return null;
            }

            return (Convert.ToUInt16(val));
            
        }

        /// <summary>
        /// Set HrmAntId field</summary>
        /// <param name="hrmAntId_">Nullable field value to be set</param>
        public void SetHrmAntId(ushort? hrmAntId_)
        {
            SetFieldValue(1, 0, hrmAntId_, Fit.SubfieldIndexMainField);
        }
        
        ///<summary>
        /// Retrieves the LogHrv field</summary>
        /// <returns>Returns nullable Bool enum representing the LogHrv field</returns>
        public Bool? GetLogHrv()
        {
            object obj = GetFieldValue(2, 0, Fit.SubfieldIndexMainField);
            Bool? value = obj == null ? (Bool?)null : (Bool)obj;
            return value;
        }

        /// <summary>
        /// Set LogHrv field</summary>
        /// <param name="logHrv_">Nullable field value to be set</param>
        public void SetLogHrv(Bool? logHrv_)
        {
            SetFieldValue(2, 0, logHrv_, Fit.SubfieldIndexMainField);
        }
        
        ///<summary>
        /// Retrieves the HrmAntIdTransType field</summary>
        /// <returns>Returns nullable byte representing the HrmAntIdTransType field</returns>
        public byte? GetHrmAntIdTransType()
        {
            Object val = GetFieldValue(3, 0, Fit.SubfieldIndexMainField);
            if(val == null)
            {
                return null;
            }

            return (Convert.ToByte(val));
            
        }

        /// <summary>
        /// Set HrmAntIdTransType field</summary>
        /// <param name="hrmAntIdTransType_">Nullable field value to be set</param>
        public void SetHrmAntIdTransType(byte? hrmAntIdTransType_)
        {
            SetFieldValue(3, 0, hrmAntIdTransType_, Fit.SubfieldIndexMainField);
        }
        
        #endregion // Methods
    } // Class
} // namespace
