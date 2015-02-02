using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class MobileSync
    {
        IntPtr sd;
        object sync = new object();

        public MobileSync(IntPtr sd)
        {
            this.sd = sd;
        }
    }
}
