using System;
using System.Runtime.InteropServices;

namespace Driver;

[StructLayout(LayoutKind.Sequential)]
public struct SysInfo
{
    public long uptime;            
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public ulong[] loads;           
    public ulong totalram;          
    public ulong freeram;           
    public ulong sharedram;         
    public ulong bufferram;         
    public ulong totalswap;         
    public ulong freeswap;          
    public ushort procs;            
    public ulong totalhigh;         
    public ulong freehigh;          
    public uint mem_unit;           
}



public class CpuInfoReader
{
    [DllImport("libc")]
    private static extern int sysinfo(ref SysInfo info);
    
    public SysInfo? Read()
    {
        var info = new SysInfo();
        var ret = sysinfo(ref info);
        return ret == 0 ? info : null;
    }
}