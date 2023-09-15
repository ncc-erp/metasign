using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.SignServerWorkers.Dto
{
    public class SetWorkerPropertyDto
    {
        public int WorkerId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ConfigWorkerDto {
        public int workerId { get; set; }
        public Dictionary<string, string> propertiesAndValues { get; set; }
        public List<string> propertiesToRemove { get; set; }
}
}
