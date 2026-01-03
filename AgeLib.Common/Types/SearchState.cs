using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Common.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly struct SearchState(int local_total, int local_last, int remote_total, int remote_last)
{
    public readonly int LocalTotal = local_total;
    public readonly int LocalLast = local_last;
    public readonly int RemoteTotal = remote_total;
    public readonly int RemoteLast = remote_last;
}
