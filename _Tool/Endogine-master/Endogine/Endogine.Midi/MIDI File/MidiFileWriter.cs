/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/06/2004
 */

using System;
using System.IO;

namespace Endogine.Midi
{
	/// <summary>
	/// Writes data from a Sequence to a MIDI file.
	/// </summary>
	public class MidiFileWriter
	{
        #region Constants

        // The format maximum value.
        private const int FormatMax = 2;

        #endregion

        #region Read Only

        // File header ID.
        private static readonly byte[] FileHeaderID =
        {
            (byte)'M',
            (byte)'T',
            (byte)'h',
            (byte)'d',
            (byte) 0,
            (byte) 0,
            (byte) 0,
            (byte) 6
        };

        // Track header ID
        private static readonly byte[] TrackHeaderID =
        {
            (byte)'M',
            (byte)'T',
            (byte)'r',
            (byte)'k'
        };

        #endregion

        #region Fields

        // The MIDI file format.
        private short format = 0;

        // The sequence to write.
        private Sequence seq;

        // For writing the data as a binary file.
        private BinaryWriter writer;

        // For streaming MIDI events.
        private MemoryStream midiStream;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the MidiFileWriter class.
        /// </summary>
		public MidiFileWriter()
		{
		}

        /// <summary>
        /// Initializes a new instance of the MidiFileWriter class with the
        /// specified file path name, the MIDI file format, and the sequence
        /// to write to a MIDI file.
        /// </summary>
        /// <param name="path">
        /// The file path name to use to write the MIDI file.
        /// </param>
        /// <param name="format">
        /// The MIDI file format.
        /// </param>
        /// <param name="seq">
        /// The sequence to write as a MIDI file.
        /// </param>
        public MidiFileWriter(string path, short format, Sequence seq)
        {
            this.seq = seq;
            Format = format;

            Write(path);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the sequence to a MIDI file.
        /// </summary>
        /// <param name="path">
        /// The file path name to use to write the MIDI file.
        /// </param>
        public void Write(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            writer = new BinaryWriter(fs);

            try
            {
                WriteFileHeader();
                WriteFormat();
                WriteTrackCount();
                WriteDivision();

                // Write each track.
                for(int i = 0; i < Sequence.Count; i++)
                {
                    WriteTrack(Sequence[i]);
                }
            }
            finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Writes the file header.
        /// </summary>
        private void WriteFileHeader()
        {
            writer.Write(FileHeaderID);
        }

        /// <summary>
        /// Writes the MIDI file format.
        /// </summary>
        private void WriteFormat()
        {
            WriteShort(Format);
        }

        /// <summary>
        /// Writes the track count.
        /// </summary>
        private void WriteTrackCount()
        {
            WriteShort((short)Sequence.Count);
        }

        /// <summary>
        /// Writes the MIDI file division.
        /// </summary>
        private void WriteDivision()
        {
            WriteShort((short)Sequence.Division);
        }

        /// <summary>
        /// Writes short values to file.
        /// </summary>
        /// <param name="value"></param>
        private void WriteShort(short value)
        {
            int v = value;

            if(BitConverter.IsLittleEndian)
            {
                v = value >> 8;
                v |= value << 8;
            }

            writer.Write((short)v);
        }

        /// <summary>
        /// Writes the specified track to MIDI file.
        /// </summary>
        /// <param name="trk">
        /// The track to write to the MIDI file.
        /// </param>
        private void WriteTrack(Track trk)
        {
            writer.Write(TrackHeaderID);

            //
            // The length of the track in bytes will be written to the 
            // MIDI file first. However, at this point, the length is unknown.
            //
            // The track is written to a memory stream first. After this is 
            // done, the length of the track can be retrieved from the stream.
            // This value is written to the MIDI file followed by the contents
            // of the stream.
            //

            midiStream = new MemoryStream();

            MessageWriter msgWriter = new MessageWriter(midiStream);

            // Write each MIDI event to the stream via the message writer.
            for(int i = 0; i < trk.Count; i++)
            {
                msgWriter.WriteNextEvent(trk[i]);
            }

            // Get the length of the track.
            int trackLength = (int)midiStream.Length;

            // Convert value if this platform uses the little endian format.
            if(BitConverter.IsLittleEndian)
            {
                byte[] b = BitConverter.GetBytes(trackLength);
                Array.Reverse(b);
                trackLength = BitConverter.ToInt32(b, 0);
            }

            // Write track length.
            writer.Write(trackLength);

            midiStream.Position = 0;

            // Write the contents of the stream to the MIDI file.
            for(int i = 0; i < midiStream.Length; i++)
            {
                writer.Write((byte)midiStream.ReadByte());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the MIDI file format.
        /// </summary>
        public short Format
        {
            get
            {
                return format;
            }
            set
            {
                // Enforce preconditions.
                if(value > FormatMax)
                    throw new ArgumentOutOfRangeException("Format", value,
                        "MIDI file format out of range.");
                else if(value == 0 && Sequence.Count > 1)
                    throw new ArgumentException("MIDI file format invalid.",
                        "Format");

                format = value;
            }
        }

        /// <summary>
        /// Gets or sets the sequence to write to the MIDI file.
        /// </summary>
        public Sequence Sequence
        {
            get
            {
                return seq;
            }
            set
            {
                if(value != null)
                {
                    seq = value;                    
                }
                else
                {
                    seq = new Sequence();
                }

                if(Sequence.Count <= 1)
                {
                    Format = 0;
                }
            }
        }

        #endregion
	}
}
