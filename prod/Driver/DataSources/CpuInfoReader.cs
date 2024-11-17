using System.Runtime.InteropServices;
using DataModel;

namespace Driver.DataSources;

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



public class CpuInfoReader : IDataSource 
{
    [DllImport("libc")]
    private static extern int sysinfo(ref SysInfo info);
    
    public SysInfo? Read()
    {
        var info = new SysInfo();
        var ret = sysinfo(ref info);
        return ret == 0 ? info : null;
    }

    public async Task<SensorMeasure[]> Read(DateTime currentTime)
    {
        var sysInfo = Read();
        if (sysInfo == null) return await Task.FromResult(Array.Empty<SensorMeasure>());

        var results = new SensorMeasure[]
        {
            new SensorMeasure(currentTime, new SensorData(SensorIds.CpuLoad1, sysInfo.Value.loads[0])),
            new SensorMeasure(currentTime, new SensorData(SensorIds.CpuLoad2, sysInfo.Value.loads[1])),
            new SensorMeasure(currentTime, new SensorData(SensorIds.CpuLoad3, sysInfo.Value.loads[2]))

        };
        return await Task.FromResult(results);

    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}