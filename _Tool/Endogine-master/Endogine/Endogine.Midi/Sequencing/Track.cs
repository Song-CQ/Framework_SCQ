/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/26/2004
 */

using System;
using System.Collections;
using System.Text;

namespace Endogine.Midi
{
	/// <summary>
	/// Represents a collection of MIDI events.
	/// </summary>
    public class Track : ICloneable, IEnumerable
    {
        #region Track Members

        #region Fields

        // The length of the track in ticks.
        private int length = 0;

        // Midi event list.
        private ArrayList midiEvents = new ArrayList();

        // Indicates whether or not the track is enabled to record MIDI events.
        private bool recordEnabled = false;  
      
        // The Colloection of objects that have locked the track to keep it from 
        // being modified.
        private ArrayList lockers = ArrayList.Synchronized(new ArrayList());

        // The current track version.
        private int version = 0;

        // The previous track version.
        private int prevVersion = 0;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Track class.
        /// </summary>
        public Track()
        {
            MetaMessage msg = new MetaMessage(MetaType.EndOfTrack, 0);
            MidiEvent e = new MidiEvent(msg, 0);
            midiEvents.Add(e);
        }       
 
        /// <summary>
        /// Initializes a new instance of the Track class with another instance
        /// of the Track class.
        /// </summary>
        /// <param name="trk">
        /// The Track instance to use for initialization.
        /// </param>
        public Track(Track trk)
        {
            // Copy events from the existing track into this one.
            foreach(MidiEvent e in trk.midiEvents)
            {
                this.midiEvents.Add(e.Clone());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a Midi event to the end of the track.
        /// </summary>
        /// <param name="e">
        /// The Midi event to add to the track.
        /// </param>
        public void Add(MidiEvent e)
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify track. It is currently locked");

            // Inserting the next MIDI event before the last event ensures
            // that the track ends with an end of track message.
            midiEvents.Insert(Count - 1, e);

            version++;
        }     
  
        /// <summary>
        /// Removes all but the last MIDI event from the track.
        /// </summary>
        /// <remarks>
        /// The very last message in a track is an end of track meta message
        /// This message must be present at the end of all tracks. When a track 
        /// is cleared, all but the last message are removed; The end of track 
        /// message is left so that the track remains valid after it has been 
        /// cleared.
        /// </remarks>
        public void Clear()
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify track. It is currently locked");

            midiEvents.Clear();
            MetaMessage msg = new MetaMessage(MetaType.EndOfTrack, 0);
            midiEvents.Add(new MidiEvent(msg, 0));

            version++;
        }
 
        /// <summary>
        /// Inserts a MidiEvent into the Track at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which <i>e</i> should be inserted. 
        /// </param>
        /// <param name="e">
        /// The MidiEvent to insert.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater than or equal to 
        /// Count.
        /// </exception>
        public void Insert(int index, MidiEvent e)
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify track. It is currently locked");
            else if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index into track out of range.");

            midiEvents.Insert(index, e);

            version++;
        }        

        /// <summary>
        /// Removes a MidiEvent at the specified index of the Track.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the MidiEvent to remove. 
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater than or equal to 
        /// Count minus one.
        /// </exception>
        /// <remarks>
        /// Every track must end with an end of track message. If an attempt is
        /// made to remove the end of track message, an exception is thrown.
        /// </remarks>
        public void RemoveAt(int index)
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify track. It is currently locked");
            else if(index < 0 || index >= Count - 1)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index into track out of range.");

            // If the event to be removed is not the last event in the track.
            if(index < Count - 1)
            {
                // Slide the event that comes immediately after the event to be
                // removed forward in time so that it remains in the same 
                // position after the previous event has been removed
                Slide(index + 1, this[index].Ticks);
            }

            // Remove event from track.
            midiEvents.RemoveAt(index);

            version++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <remarks>
        /// Every track must end with an end of track message. If an attempt is
        /// made to remove a range of events which includes the end of track 
        /// message, an exception is thrown.
        /// </remarks>
        public void RemoveRange(int index, int count)
        { 
            // Guard.
            if(count == 0)
                return;

            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify track. It is currently locked");

            // The index to the last MIDI event to remove.
            int endIndex = count - 1 + index;

            // Enforce preconditions.
            if(index < 0 || count < 0 || endIndex >= Count - 1)
                throw new ArgumentOutOfRangeException("Range invalid.");

            int slideAmount = 0;

            // Determine how far to slide the rest of the events in the 
            // track.
            for(int i = index; i <= endIndex; i++)
            {
                slideAmount += this[i].Ticks;
            }

            // Slide the event that comes immediately after the last 
            // event to be removed forward in time so that it remains 
            // in the same position after the last event has been 
            // removed.
            Slide(endIndex + 1, slideAmount);

            // Remove range of events.
            midiEvents.RemoveRange(index, count);

            version++;
        }

        /// <summary>
        /// Slides events forwards or backwards at the specified index in the 
        /// Track.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the MidiEvent to slide. 
        /// </param>
        /// <param name="slideAmount">
        /// The amount to slide the MidiEvent.
        /// </param>
        /// <remarks>
        /// If the slide amount is a negative number, the Midi event at the
        /// specified index will be moved backwards in time; its ticks value 
        /// will be summed with the slide amount thus reducing its value. It
        /// is important that using a negative slide amount does not result in
        /// a negative tick value for the specified Midi event. If this occurs,
        /// an exception is thrown. If the slide amount is positive, the Midi 
        /// event at the specified index will be moved forwards in time; its 
        /// ticks value will be increased by the slide amount. 
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is less than zero or greater than or equal to 
        /// Count. Or if slide amount results in a ticks value less than zero.
        /// </exception>
        public void Slide(int index, int slideAmount)
        {
            // Enforce preconditions.
            if(IsLocked())
                throw new InvalidOperationException(
                    "Cannot modify track. It is currently locked");
            else if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index into track out of range.");

            MidiEvent e = (MidiEvent)midiEvents[index];

            // Enforce preconditions.
            if(e.Ticks + slideAmount < 0)
                throw new ArgumentOutOfRangeException("slideAmount", slideAmount,
                    "Slide amount out of range.");
            
            // Slide MidiEvent ticks value by the slide amount.
            e.Ticks += slideAmount;

            // Put Midi event back into track;
            midiEvents[index] = e;

            version++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int IndexToPosition(int index)
        {
            // Enforce preconditions.
            if(index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index", index,
                    "Index into track out of range.");

            int position = 0;
            
            for(int i = 0; i <= index; i++)
            {
                position += this[i].Ticks;
            }

            return position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int PositionToIndex(int position)
        {
            // Enforce preconditions.
            if(position < 0)
                throw new ArgumentOutOfRangeException("position", position,
                    "Position into track out of range.");

            int index = 0;
            int ticks = 0;

            while(index < Count && ticks < position)
            {
                ticks += this[index].Ticks;

                if(ticks < position)
                {
                    index++;
                }
            }

            if(index >= Count)
            {
                index = -1;
            }

            return index;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRecordEnabled()
        {
            return RecordEnabled;
        }

        internal void LockTrack(object locker, bool lockTrack)
        {
            if(lockTrack)
                lockers.Add(locker);
            else
                lockers.Remove(locker);
        }

        internal bool IsLocked()
        {
            return lockers.Count > 0;
        }

        /// <summary>
        /// Merges two tracks together.
        /// </summary>
        /// <param name="trackA">
        /// The first of two tracks to merge.
        /// </param>
        /// <param name="trackB">
        /// The second of two tracks to merge.
        /// </param>
        /// <returns>
        /// The merged track.
        /// </returns>
        public static Track Merge(Track trackA, Track trackB)
        {
            Track trkA = new Track(trackA);
            Track trkB = new Track(trackB);
            Track mergedTrack = new Track();
            int a = 0, b = 0; 

            //
            // The following algorithm merges two Midi tracks together. It 
            // assumes that both tracks are valid in that both end with a
            // end of track meta message.
            //

            // While neither the end of track A or track B has been reached.
            while(a < trkA.Count - 1 && b < trkB.Count - 1)
            {
                // While the end of track A has not been reached and the 
                // current Midi event in track A comes before the current Midi
                // event in track B.
                while(a < trkA.Count - 1 && trkA[a].Ticks <= trkB[b].Ticks)
                {
                    // Slide the events in track B backwards by the amount of
                    // ticks in the current event in track A. This keeps both
                    // tracks in sync.
                    trkB.Slide(b, -trkA[a].Ticks);

                    // Add the current event in track A to the merged track.
                    mergedTrack.Add(trkA[a]);

                    // Move to the next Midi event in track A.
                    a++;
                }

                // If the end of track A has not yet been reached.
                if(a < trkA.Count - 1)
                {
                    // While the end of track B has not been reached and the 
                    // current Midi event in track B comes before the current Midi
                    // event in track A.
                    while(b < trkB.Count - 1 && trkB[b].Ticks < trkA[a].Ticks)
                    {
                        // Slide the events in track A backwards by the amount of
                        // ticks in the current event in track B. This keeps both
                        // tracks in sync.
                        trkA.Slide(a, -trkB[b].Ticks);

                        // Add the current event in track B to the merged track.
                        mergedTrack.Add(trkB[b]);

                        // Move forward to the next Midi event in track B.
                        b++;
                    }
                }
            }
            
            // If the end of track A has not yet been reached.
            if(a < trkA.Count - 1)
            {
                // Add the rest of the events in track A to the merged track.
                while(a < trkA.Count - 1)
                {
                    mergedTrack.Add(trkA[a]);
                    a++;
                }
            }
                // Else if the end of track B has not yet been reached.
            else if(b < trkB.Count - 1)
            {
                // Add the rest of the events in track B to the merged track.
                while(b < trkB.Count - 1)
                {
                    mergedTrack.Add(trkB[b]);
                    b++;
                }
            }
            
            return mergedTrack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        /// <returns></returns>
        public static Track Merge(ArrayList tracks)
        {
            Track mergedTrack = new Track();
            Track currentTrack;
            ArrayList trackList = new ArrayList();
            ArrayList events = new ArrayList();
            ArrayList trackIndexes = new ArrayList();

            for(int i = 0; i < tracks.Count; i++)
            {
                currentTrack = (Track)tracks[i];

                if(currentTrack.Count > 1)
                {
                    trackList.Add(currentTrack);
                    trackIndexes.Add(0);
                    events.Add(currentTrack[0]);
                }
            }

            while(events.Count > 0)
            {
                int n = 0;
                MidiEvent e1 = (MidiEvent)events[0];
                MidiEvent e2;
                int ticks = e1.Ticks;
          
                for(int i = 1; i < events.Count; i++)
                {
                    e1 = (MidiEvent)events[i];

                    if(e1.Ticks < ticks)
                    {
                        ticks = e1.Ticks;
                        n = i;
                    }
                }

                e1 = (MidiEvent)events[n];
                mergedTrack.Add(e1);

                for(int i = 0; i < events.Count; i++)
                {
                    e2 = (MidiEvent)events[i];
                    e2.Ticks -= e1.Ticks;
                    events[i] = e2;
                }

                int counter = (int)trackIndexes[n] + 1;

                currentTrack = (Track)trackList[n];

                if(counter < currentTrack.Count - 1)
                {
                    events[n] = currentTrack[counter];
                    trackIndexes[n] = counter;
                }
                else
                {
                    trackList.RemoveAt(n);
                    trackIndexes.RemoveAt(n);
                    events.RemoveAt(n);                    
                }
            }

            return mergedTrack;
        }
    
        /// <summary>
        /// Finds the track name meta message.
        /// </summary>
        /// <returns>
        /// The index to the MIDI event containing the track name meta message
        /// if it exists; otherwise, -1.
        /// </returns>
        private int FindTrackNameMetaMessage()
        {
            int count = Count - 1;

            for(int i = 0; i < count; i++)
            {
                if(IsTrackNameMetaMessage(this[i].Message))
                    return i;
            }

            return -1;
        }

        private bool IsTrackNameMetaMessage(IMidiMessage message)
        {
            // Guard.
            if(!(message is MetaMessage))
                return false;

            MetaMessage msg = (MetaMessage)message;

            if(msg.Type == MetaType.TrackName)
                return true;
            else
                return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the MidiEvent at the specified index.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public MidiEvent this[int index]
        {
            get
            {
                // Enforce preconditions.
                if(index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into track out of range.");

                return (MidiEvent)midiEvents[index];
            }
            set
            {
                // Enforce preconditions.
                if(index < 0 || index >= Count - 1)
                    throw new ArgumentOutOfRangeException("index", index,
                        "Index into track out of range.");

                MidiEvent e = value;

                midiEvents[index] = e;
            }
        }

        /// <summary>
        /// Gets the number of MidiEvents in the track.
        /// </summary>
        public int Count
        {
            get
            {
                return midiEvents.Count;
            }
        }

        /// <summary>
        /// Gets or sets the track name.
        /// </summary>
        /// <remarks>
        /// If a track name does not exist, an empty string is returned.
        /// </remarks>
        public string Name
        {
            get
            {
                string name = string.Empty;
                int index = FindTrackNameMetaMessage();

                // If a track name meta message exists.
                if(index >= 0)
                {
                    MetaMessage msg = (MetaMessage)this[index].Message;
                    MetaMessageText msgText = new MetaMessageText(msg);
                    name = msgText.Text;
                }

                return name;
            }
            set
            {
                // Enforce preconditions.
                if(IsLocked())
                    throw new InvalidOperationException(
                        "Cannot modify track. It is currently locked");

                int index = FindTrackNameMetaMessage();

                // If the track name meta message exists.
                if(index >= 0)
                {    
                    MidiEvent e = this[index];
                    MetaMessage msg = (MetaMessage)e.Message;
                    MetaMessageText msgText = new MetaMessageText(msg);

                    msgText.Text = value;

                    e.Message = msgText.ToMessage();

                    this[index] = e;
                }
                // Else the track name meta message does not exist.
                else
                {
                    // Add new meta message for the track name.
                    MetaMessageText msgText = new MetaMessageText();

                    msgText.Text = value;
                    Insert(0, new MidiEvent(msgText.ToMessage(), 0));
                }
            }
        }

        /// <summary>
        /// Gets the length of the track in ticks.
        /// </summary>
        public int Length
        {
            get
            {
                if(prevVersion != Version)
                {
                    length = 0;

                    // Calculate the length of the track by summing the ticks value of 
                    // every Midi event in the track.
                    foreach(MidiEvent e in midiEvents)
                    {
                        length += e.Ticks;
                    }

                    prevVersion = Version;
                }

                return length;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the track is enabled to
        /// record MIDI events.
        /// </summary>
        internal bool RecordEnabled
        {
            get
            {
                return recordEnabled;
            }
            set
            {
                recordEnabled = value;
            }
        }

        /// <summary>
        /// Gets a value representing the version of the track.
        /// </summary>
        internal int Version
        {
            get
            {
                return version;
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the Track.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this Track.
        /// </returns>
        public object Clone()
        {
            return new Track(this);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that can iterate through the track's MIDI
        /// events.
        /// </summary>
        /// <returns>
        /// An enumerator that can iterate through the track's MIDI events.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new TrackEnumerator(this);
        }

        #endregion

        #region TrackEnumerator Class

        /// <summary>
        /// Provides enumeration for the Track class.
        /// </summary>
        private class TrackEnumerator : IEnumerator
        {
            #region TrackEnumerator Members

            #region Fields

            // The track to iterate over.
            private Track owner;

            // The track version - used to make sure the track has not been
            // modified since the creation of the enumerator.
            private int version;

            // MIDI event index.
            private int eventIndex = -1;

            // The MIDI event at the current position.
            private MidiEvent currentEvent;

            #endregion

            #region Construction

            /// <summary>
            /// Initializes a new instance of the TrackEnumerator class with 
            /// the specified track to iterate over.
            /// </summary>
            /// <param name="owner">
            /// The track to iterate over.
            /// </param>
            public TrackEnumerator(Track owner)
            {
                this.owner = owner;
                version = owner.Version;
            }

            #endregion

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Moves to the next MIDI event in the track.
            /// </summary>
            /// <returns>
            /// <b>true</b> if the end of the track has not yet been reached; 
            /// otherwise, <b>false</b>.
            /// </returns>
            public bool MoveNext()
            {
                // Enforce preconditions.
                if(version != owner.Version)
                    throw new InvalidOperationException(
                        "The track was modified after the enumerator was created.");

                // Move to the next event in the track.
                eventIndex++;

                // If the end of the track has not been reached.
                if(eventIndex < owner.Count)
                {
                    // Get the event at the current position.
                    currentEvent = owner[eventIndex];

                    // Indicate that the end of the track has not yet been 
                    // reached.
                    return true;
                }
                // Else the end of the track has been reached.
                else
                {
                    // Indicate that the end of the track has been reached.
                    return false;
                }
            }

            /// <summary>
            /// Resets the enumerator to just before the beginning of the 
            /// track.
            /// </summary>
            public void Reset()
            {
                // Enforce preconditions.
                if(version != owner.Version)
                    throw new InvalidOperationException(
                        "The track was modified after the enumerator was created.");

                // Reset position to just before the beginning of the track.
                eventIndex = -1;
            }

            /// <summary>
            /// Gets the MIDI event at the current position in the track.
            /// </summary>
            public object Current
            {
                get
                {
                    // Enforce preconditions.
                    if(eventIndex < 0 || eventIndex >= owner.Count)
                        throw new InvalidOperationException(
                            "The enumerator is positioned before the first " +
                            "event of the track or after the last event.");

                    return currentEvent;
                }
            }        

            #endregion
        }

        #endregion
    }
}
