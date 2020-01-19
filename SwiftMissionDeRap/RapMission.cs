using System.IO;
using System.Text;

namespace SwiftMissionDeRap
{
    public class RapMission : RapClass
    {
        string Tag;
        ulong Always0;
        ulong Always8;
        ulong OffsetToEnums;
        public RapMission(FileInfo file)
        {
            using(stream = file.OpenRead())
            {
                // skip tag
                if(stream.ReadByte() != 0x0)
                    throw new System.Exception("Not a proper rap sqm");
                var tagBuffer = new byte[3];
                var read = stream.Read(tagBuffer, 0, 3);
                Tag = Encoding.ASCII.GetString(tagBuffer);
                if (Tag != "raP")
                   throw new System.Exception("Not a proper rap sqm");
                Always0 = ReadInt32();
                Always8 = ReadInt32();
                OffsetToEnums = ReadInt32();
                pos = stream.Position;
                Read();
            }
        }
    }
}
