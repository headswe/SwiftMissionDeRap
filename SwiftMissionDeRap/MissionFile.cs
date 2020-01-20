using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftMissionDeRap
{
    public class MissionFile : TextClass
    {
        public MissionFile(FileInfo file)
        {
            using(var reader = file.OpenText())
            {
                readClass(reader.ReadToEnd());

            }

        }

    }
}
