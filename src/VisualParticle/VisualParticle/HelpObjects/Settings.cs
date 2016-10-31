using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SoundCollector.HelpObjects
{
    [Serializable()]
    public class Settings : ISerializable
    {
        public Int32 WindowWidth { get; set; }

        public Int32 WindowHeight { get; set; }

        public Settings()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("SerializationInfo is null");

            info.AddValue("WindowWidth", this.WindowWidth);
            info.AddValue("WindowHeight", this.WindowHeight);
        }
    }
}
