using System;
using System.Collections.Generic;
using System.Text;

namespace NccCore.BackgroundJob
{
    public class EmailBackgroundJobArgs
    {
        public List<string> TargetEmails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
