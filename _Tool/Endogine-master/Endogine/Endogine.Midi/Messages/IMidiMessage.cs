/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 08/05/2004
 */

using System;

namespace Endogine.Midi
{
    /// <summary>
    /// Represents the basic functionality for all MIDI messages.
    /// </summary>
    public interface IMidiMessage : ICloneable
    {  
        /// <summary>
        /// Accepts a MIDI message visitor for double dispatching.
        /// </summary>
        /// <param name="visitor">
        /// The MIDI message visitor to visit.
        /// </param>
        void Accept(MidiMessageVisitor visitor);

        /// <summary>
        /// Gets the MIDI message's status value.
        /// </summary>
        int Status
        {
            get;
        }
    }
}
