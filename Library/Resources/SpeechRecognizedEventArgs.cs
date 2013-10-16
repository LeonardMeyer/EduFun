using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduFun.Library.Resources
{

    public class SpeechRecognizedEventArgs
    {
        public string result;

        public SpeechRecognizedEventArgs(string result)
        {
            this.result = result;
        }
    }
}
