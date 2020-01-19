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
            RapMission file = new RapMission(new FileInfo(args[0]));
            var str = JsonConvert.SerializeObject(file);
            File.WriteAllText("mission.json", str);
        }
    }
}
