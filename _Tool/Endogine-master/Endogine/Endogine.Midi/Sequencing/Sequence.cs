/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 11/01/2004
 */

using System;
using System.Collections;

namespace Endogine.Midi
{
	/// <summary>
	/// Represents a collection of Tracks.
	/// </summary>
	public class Sequence : IEnumerable
	{
        #region Sequence Members

        #region Constants

        // Default division resolution.
        private const int DefaultDivision = 96;

        // Number of bits to shift for splitting division value.
        private const int DivisionShift = 8;

        #endregion

        #region Fields

        // The resolution of the sequence.
        private int division;

        // The length of the sequence in ticks.
        private int length = 0;

        // The collection of tracks for the sequence.
        private ArrayList tracks = new ArrayList();

        // The collection of objects that have locked the sequence to keep it
        // from being modified.
        private ArrayList lockers = ArrayList.Synchronized(new ArrayList());

        // The current sequence version.
        private int version = 0;

        // The previous sequence version.
        private int prevVersion = 0;

        #endregion        

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Sequence class.
        /// </summary>
        public Sequence()
        {
            division = DefaultDivision;
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with the 
        /// specified division.
        /// </summary>
        /// <param name="division">
        /// The division value for the sequence.
        /// </param>
		public Sequence(int division)
		{
            this.division = division;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a track to the Sequence.
        /// </summary>
        /// <param name="trk">
        /// The track to add to the Sequence.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the sequence is currently locked.
        /// </exception>
        public void Add(Track trk)
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify sequence. It is currently locked.");

            tracks.Add(new DictionaryEntry(trk, trk.Version));

            version++;
        }

        /// <summary>
        /// Removes the specified track from the sequence.
        /// </summary>
        /// <param name="trk">
        /// The track to remove.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the sequence is currently locked.
        /// </exception>
        public void Remove(Track trk)
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify sequence. It is currently locked.");

            int i = 0;

            while(i < tracks.Count)
            {
                DictionaryEntry de = (DictionaryEntry)tracks[i];

                if((Track)de.Key == trk)
                {
                    tracks.RemoveAt(i);
                    break;
                }
                else
                {
                    i++;
                }
            }

            version++;
        }

        /// <summary>
        /// Remove the Track at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Track to remove. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the sequence is currently locked.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater or equal to Count.
        /// </exception>
        public void RemoveAt(int index)
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify sequence. It is currently locked.");
            else if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index for removing track from sequence out of range.");

            tracks.RemoveAt(index);

            version++;
        }

        /// <summary>
        /// Determines whether or not the sequence is locked.
        /// </summary>
        /// <returns>
        /// <b>true</b> if the sequence is locked; otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        /// A sequence is locked when a sequencer is playing it. Attempting to
        /// change a sequence while it is locked will result in an exception 
        /// being thrown.
        /// </remarks>
        public bool IsLocked()
        {
            return lockers.Count > 0;
        } 

        /// <summary>
        /// Determines whether or not this is a Smpte sequence.
        /// </summary>
        /// <returns>
        /// <b>true</b> if this is a Smpte sequence; otherwise, <b>false</b>.
        /// </returns>
        public bool IsSmpte()
        {
            bool result = false;

            // The upper byte of the division value will be negative if this is
            // a SMPTE sequence.
            if((sbyte)(division >> DivisionShift) < 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Locks a sequence to keep it from being modified.
        /// </summary>
        /// <param name="locker">
        /// The object locking the sequence.
        /// </param>
        /// <param name="lockSequence">
        /// A value indicating whether or not to lock the sequence.
        /// </param>
        internal void LockSequence(object locker, bool lockSequence)
        {
            if(lockSequence)
                lockers.Add(locker);
            else
                lockers.Remove(locker);

            foreach(DictionaryEntry de in tracks)
            {
                Track t = (Track)de.Key;
                t.LockTrack(locker, lockSequence);
            }
        }
        
        #endregion
   
        #region Properties

        /// <summary>
        /// Gets or sets the Track at the specified index.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if an attempt is made to set a track at the specified index
        /// and the sequence is currently locked.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater or equal to Count.
        /// </exception>
        public Track this[int index]
        {
            get
            {
                // Enforce preconditions.
                if(index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into sequence out of range.");

                DictionaryEntry de = (DictionaryEntry)tracks[index];

                return (Track)de.Key;
            }
            set
            {
                // Enforce preconditions.
                if(IsLocked())
                    throw new InvalidOperationException(
                        "Cannot modify sequence. It is currently locked.");
                else if(index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into sequence out of range.");                

                tracks[index] = new DictionaryEntry(value, value.Version);

                version++;
            }
        }

        /// <summary>
        /// Gets the number of Tracks in the Sequence.
        /// </summary>
        public int Count
        {
            get
            {
                return tracks.Count;
            }
        }

        /// <summary>
        /// Gets the division value for the Sequence.
        /// </summary>
        public int Division
        {
            get
            {
                return division;
            }
        }

        /// <summary>
        /// Gets the length of the sequence in ticks.
        /// </summary>
        public int Length
        {
            get
            {
                if(prevVersion != Version)
                {
                    length = 0;

                    foreach(DictionaryEntry de in tracks)
                    {
                        Track t = (Track)de.Key; 

                        if(length < t.Length)
                            length = t.Length;
                    }

                    prevVersion = Version;
                }

                return length;
            }
        }

        /// <summary>
        /// Gets a value representing the version of the sequence.
        /// </summary>
        internal int Version
        {
            get
            {
                for(int i = 0; i < tracks.Count; i++)
                {
                    DictionaryEntry de = (DictionaryEntry)tracks[i];
                    Track t = (Track)de.Key;

                    if(t.Version != (int)de.Value)
                    {
                        tracks[i] = new DictionaryEntry(t, t.Version);
                        version++;
                    }
                }

                return version;
            }
        }

        #endregion

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new SequenceEnumerator(this);
        }

        #endregion

        #region SequenceEnumerator Class

        /// <summary>
        /// Provides enumeration for the Sequence class.
        /// </summary>
        private class SequenceEnumerator : IEnumerator
        {
            #region SequenceEnumerator Members

            #region Fields

            // The sequence to iterate over.
            private Sequence owner;

            // The sequence version - used to make sure the sequence has not 
            // been modified since the creation of the enumerator.
            private int version;

            // Track index.
            private int trackIndex = -1;

            #endregion

            #region Construction

            /// <summary>
            /// Initializes a new instance of the SequenceEnumerator class with 
            /// the specified sequence to iterate over.
            /// </summary>
            /// <param name="owner">
            /// The sequence to iterate over.
            /// </param>
            public SequenceEnumerator(Sequence owner)
            {
                this.owner = owner;
                this.version = owner.version;
            }

            #endregion

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Resets the enumerator to just before the first track in the
            /// sequence.
            /// </summary>
            public void Reset()
            {
                // Enforce preconditions.
                if(version != owner.version)
                    throw new InvalidOperationException(
                        "The sequence was modified after the enumerator was created.");

                trackIndex = -1;
            }

            /// <summary>
            /// Moves to the next track in the sequence.
            /// </summary>
            /// <returns>
            /// <b>true</b> if the end of the sequence has not been reached; 
            /// otherwise, <b>false</b>.
            /// </returns>
            public bool MoveNext()
            {
                // Enforce preconditions.
                if(version != owner.version)
                    throw new InvalidOperationException(
                        "The sequence was modified after the enumerator was created.");

                trackIndex++;

                if(trackIndex < owner.Count)
                    return true;
                else
                    return false;
            }

            /// <summary>
            /// The track at the current position.
            /// </summary>
            public object Current
            {
                get
                {
                    // Enforce precondition.
                    if(trackIndex < 0 || trackIndex >= owner.Count)
                        throw new InvalidOperationException(
                            "The enumerator is positioned before the first " +
                            "track of the sequence or after the last track.");

                    return owner[trackIndex];
                }
            }            

            #endregion
        }

        #endregion
    }
}
