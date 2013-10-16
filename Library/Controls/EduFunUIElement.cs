using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EduFun.Library.Controls
{
    public interface EduFunUIElement
    {
        TouchState State  { get; set; }
    }
}
