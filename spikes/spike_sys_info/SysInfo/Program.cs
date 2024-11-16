using System;
using System.Runtime.InteropServices;

class Program
{
    [StructLayout(LayoutKind.Sequential)]
    struct SysInfo
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

    [DllImport("libc")]
    private static extern int sysinfo(ref SysInfo info);

    static void Main(string[] args)
    {
        SysInfo info = new SysInfo();

        if (sysinfo(ref info) == 0)
        {
            Console.WriteLine($"Uptime: {info.uptime} seconds");
            Console.WriteLine($"Load averages: {info.loads[0]} {info.loads[1]} {info.loads[2]}");
            Console.WriteLine($"Total RAM: {info.totalram * info.mem_unit / (1024 * 1024)} MB");
            Console.WriteLine($"Free RAM: {info.freeram * info.mem_unit / (1024 * 1024)} MB");
            Console.WriteLine($"Total Swap: {info.totalswap * info.mem_unit / (1024 * 1024)} MB");
            Console.WriteLine($"Free Swap: {info.freeswap * info.mem_unit / (1024 * 1024)} MB");
            Console.WriteLine($"Number of processes: {info.procs}");
        }
        else
        {
            Console.WriteLine("Error calling sysinfo");
        }
    }
}
