using System;
using System.Runtime.Serialization;

namespace EduFun.Kinect
{
    public class KinectExceptions : Exception
    {
        public KinectExceptions() : base() {}
        public KinectExceptions(String message) : base(message) {}
        public KinectExceptions(String message, Exception inner) : base(message, inner) {}

        protected KinectExceptions(SerializationInfo info,
        StreamingContext context) : base(info, context) {}
    }
}
