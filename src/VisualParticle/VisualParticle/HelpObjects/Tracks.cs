using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SoundCollector.HelpObjects
{
    [Serializable()]
    public class Tracks : ISerializable
    {
        public List<Track> TrackList { get; set; }

        public Tracks()
        {
            this.TrackList = new List<Track>();
        }

        public void Add(Track t)
        {
            this.TrackList.Add(t);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("SerializationInfo is null");

            info.AddValue("TrackList", this.TrackList);
        }
    }
}
