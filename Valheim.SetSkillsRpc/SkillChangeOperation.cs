using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valheim.SetSkillsRpc
{
    public class SkillChangeOperation
    {
        public long PlayerID { get; set; }
        public string Operation { get; set; }
        public string[] SkillNames { get; set; }
        public string[] SkillValues { get; set; }
    }
}
