using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class LockdownException : Exception
    {
        public LockdownException(string message)
            : base(message)
        {

        }
    }
}
