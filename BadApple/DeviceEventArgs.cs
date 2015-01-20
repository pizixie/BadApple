using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class DeviceEventArgs:EventArgs
    {
        public IntPtr Device { get; set; }
    }
}
