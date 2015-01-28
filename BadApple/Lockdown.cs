using BadApple.Native;
using CoreFoundation;
using PList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class Lockdown
    {
        IntPtr device;

        public Lockdown(IntPtr device)
        {
            this.device = device;
        }

        public void Connect()
        {
            int op_code;

            op_code = iTunesMobileDevice.AMDeviceConnect(this.device);

            if (op_code == 1)
                throw new Exception("Phone in recovery mode, support not yet implemented");

            op_code = iTunesMobileDevice.AMDeviceIsPaired(this.device);

            if (op_code == 0)
                throw new Exception("未配对");

            op_code = iTunesMobileDevice.AMDeviceValidatePairing(this.device);

            if (op_code != 0)
                throw new Exception("配对失败");
        }

        public void StartSession()
        {
            var op_code = iTunesMobileDevice.AMDeviceStartSession(this.device);

            if (op_code == 1)
                throw new Exception("无法启动回话");
        }

        public void StopSession()
        {
            var i = iTunesMobileDevice.AMDeviceStopSession(this.device);

            Debug.Assert(i == 0);
        }

        public void Disconnect()
        {
            var i = iTunesMobileDevice.AMDeviceDisconnect(this.device);

            Debug.Assert(i == 0);
        }

        public IntPtr StartService(string name)
        {
            this.Connect();
            this.StartSession();

            CFString cf_name = new CFString(name);

            IntPtr sd = IntPtr.Zero;

            var i = iTunesMobileDevice.AMDeviceStartService(this.device, cf_name.Handle, out sd, IntPtr.Zero);

            if (i != 0)
                throw new LockdownException("无法启动服务");

            this.StopSession();
            this.Disconnect();

            return sd;
        }

        public string GetValue(string domain, string key)
        {
            CFString cf_domain = new CFString(domain);
            CFString cf_key = new CFString(key);

            var cf_value = iTunesMobileDevice.AMDeviceCopyValue(device, cf_domain.Handle, cf_key.Handle);

            var value = new CFString(cf_value);

            return value;
        }

        public string GetValue(string key)
        {
            this.Connect();
            this.StartSession();


            CFString cf_key = new CFString(key);

            var cf_value = iTunesMobileDevice.AMDeviceCopyValue(device, IntPtr.Zero, cf_key.Handle);

            var value = new CFString(cf_value);

            this.StopSession();
            this.Disconnect();

            return value;
        }
    }

    public class LockdownException : Exception
    {
        public LockdownException(string message)
            : base(message)
        {

        }
    }
}
