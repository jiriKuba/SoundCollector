using System;
using System.Runtime.Serialization;

namespace SoundCollector.HelpObjects
{
    [Serializable()]
    public class Track : ISerializable
    {
        public String Stat { get; set; }
        public String Artis { get; set; }
        public String Name { get; set; }
        public Int32 Score { get; set; }

        private const Int32 MAX_TEXT_NAME_LENGTH = 16;
        private const String DOTS = "...";

        public Track()
        {

        }

        public Track(String stat, String artis, String name, Int32 score)
            : this()
        {
            this.Stat = stat;
            this.Artis = artis;
            this.Name = name;
            this.Score = score;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("SerializationInfo is null");

            info.AddValue("Stat", this.Stat);
            info.AddValue("Artis", this.Artis);
            info.AddValue("Name", this.Name);
            info.AddValue("Score", this.Score);
        }

        public override String ToString()
        {
            String art = (String.IsNullOrEmpty(this.Artis) || this.Artis.Length <= MAX_TEXT_NAME_LENGTH) ? this.Artis : this.Artis.Substring(0, MAX_TEXT_NAME_LENGTH) + DOTS;
            String nam = (String.IsNullOrEmpty(this.Name) || this.Name.Length <= MAX_TEXT_NAME_LENGTH) ? this.Name : this.Name.Substring(0, MAX_TEXT_NAME_LENGTH) + DOTS; ;
            return String.Format("{0} - {1} : {2}({3})", art, nam, this.Score, this.Stat);
        }
    }
}
