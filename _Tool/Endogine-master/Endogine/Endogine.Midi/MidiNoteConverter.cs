/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/21/2004
 */

using System;

namespace Endogine.Midi
{
	/// <summary>
	/// Converts a Midi note number to its corresponding frequency.
	/// </summary>
	public sealed class MidiNoteConverter
	{
        // Note per octave.
        private const int NotePerOctave = 12;

        // Offsets the note number.
        private const int NoteOffset = 9;

        // Reference frequency used for calculations.
        private const double ReferenceFrequency = 13.75;

        // Prevents instances of this class from being created - no need for
        // an instance to be created since this class only has static methods.
		private MidiNoteConverter()
		{
		}

        /// <summary>
        /// Converts note to frequency.
        /// </summary>
        /// <param name="noteNumber">
        /// The number of the note to convert.
        /// </param>
        /// <returns>
        /// The frequency of the specified note.
        /// </returns>
        public static double NoteToFrequency(int noteNumber)
        {
            double exponent = (double)(noteNumber - NoteOffset) / NotePerOctave;

            return ReferenceFrequency * Math.Pow(2.0, exponent);
        }
	}
}
