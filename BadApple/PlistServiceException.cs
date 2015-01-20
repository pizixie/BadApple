using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class PlistServiceException : Exception
    {
        public PlistServiceException(string msg)
            : base(msg)
        {

        }
    }
}
