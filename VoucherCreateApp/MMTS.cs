using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VoucherCreateApp
{
    class MMTS
    {
        public static dynamic GetConnectionProperty(String strName)
        {
            int lProc = 0;
            lProc = Thread.CurrentThread.ManagedThreadId;
            PropsMgr.ShareProps spmMgr = new PropsMgr.ShareProps();
            dynamic prop = spmMgr.GetProperty(lProc,strName);
            return  prop;
        }

        public static String PropsString()
        {
            return GetConnectionProperty("PropsString");
        }
    }
}
