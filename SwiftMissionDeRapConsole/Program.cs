using Newtonsoft.Json;
using SwiftMissionDeRap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftMissionDeRapConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MissionFile sfile = new MissionFile(new FileInfo("vmission.sqm"));
           RapifiedMissionFile mfile = new RapifiedMissionFile(new FileInfo(args[0]));
            var str = JsonConvert.SerializeObject(sfile);
            File.WriteAllText("mission.json", str);
            str = JsonConvert.SerializeObject(mfile);
            File.WriteAllText("mission_bin.json", str);
            //  
        }
    }
}
