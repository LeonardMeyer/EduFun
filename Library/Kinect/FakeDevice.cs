using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EduFun.Kinect
{
    class FakeDevice : TouchDevice
    {

        public FakeDevice(int deviceId, Touch toucher, Size WindowSize, TouchAction toucherAction) : base(deviceId)
        {
            touch = toucher;
            TargetWindowSize = WindowSize;
            touchAction = toucherAction;            
        }

        public Touch touch { get; set; }
        public Size TargetWindowSize { get; set; }
        public TouchAction touchAction { get; set; }

        public override TouchPointCollection GetIntermediateTouchPoints(System.Windows.IInputElement relativeTo)
        {
            return null;
        }

        public override TouchPoint GetTouchPoint(System.Windows.IInputElement relativeTo)
        {
            return new TouchPoint(this, touch.GetScreenPositionPixels(TargetWindowSize), new Rect(), TouchAction.Up);
        }
    }
}
