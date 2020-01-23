using System;

namespace Sysinfo
{
    [Serializable]
    public class Storage // stores the datas of the processes
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public long MemoryUsage { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan RunningTime { get; set; }
        public int ThreadCount { get; set; }
        public string Comment { get; set; }
        
        public Storage()
        {

        }
    }
}
