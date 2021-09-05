/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 02/18/2004
 */

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Endogine.Midi
{
    /// <summary>
    /// Represents the Windows Multimedia MidiHDR structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiHeader
    {
        #region MidiHeader Members

        public IntPtr        data;
        public int           bufferLength; 
        public int           bytesRecorded; 
        public int           user; 
        public int           flags; 
        public IntPtr        next; 
        public int           reserved; 
        public int           offset; 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
        public int[]         reservedArray; 

        #endregion
    }
}
