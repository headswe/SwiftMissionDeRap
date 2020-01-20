using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftMissionDeRap
{
    public class SqmClass
    {
        public string Name { get; set; }
        public Dictionary<string, dynamic> Attributes { get; set; } = new Dictionary<string, dynamic>();
        public Dictionary<string, SqmClass> Children { get; set; } = new Dictionary<string, SqmClass>();
    }
}
