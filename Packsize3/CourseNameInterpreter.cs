using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packsize3
{
    public class CourseNameInterpreter : NameInterpreter
    {
        public override char GetDelimiter()
        {
            return ':';
        }
    }
}
