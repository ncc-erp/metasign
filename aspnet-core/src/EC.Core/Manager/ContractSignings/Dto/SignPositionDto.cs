using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager.ContractSignings.Dto
{
    public class SignPositionDto
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int Page { get; set; }
    }
}
