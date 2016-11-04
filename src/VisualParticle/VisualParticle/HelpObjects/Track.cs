using SoundCollector.Utils;
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

        public const Int32 MAX_TEXT_NAME_LENGTH = 18;
        public const Int32 MAX_FULL_TEXT_LENGTH = 37;
        public const String DOTS = "...";

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
            String songName = TextUtils.GetSongName(this.Artis, this.Name);
            songName = (String.IsNullOrEmpty(songName) || songName.Length <= MAX_TEXT_NAME_LENGTH) ? songName : songName.Substring(0, MAX_TEXT_NAME_LENGTH) + Track.DOTS;
            songName = TextUtils.RemoveDiacritics(songName);
            return String.Format("{0} : {1}({2})", songName, this.Score, this.Stat);
        }
    }
}