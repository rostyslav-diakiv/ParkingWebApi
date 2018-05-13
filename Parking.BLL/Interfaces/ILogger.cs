using System;

namespace Parking.BLL.Interfaces
{
    public interface ILogger
    {
        void DumpLog();
        void LogError(Exception ex);
        void LogError(string text);
        void LogInfo(string info, DateTime time);
    }
}