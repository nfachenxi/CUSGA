using System;

namespace Common.Utils
{
    public class TimeUtil
    {
        public static double timestamp
        {
            get { return GetTimestamp(DateTime.Now); }
        }

        public static DateTime GetTime(long timeStamp)
        {
            // 使用UTC时间作为基准点
            DateTime startTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // UTC时间转本地时间
            DateTime dateTimeStart = TimeZoneInfo.ConvertTimeFromUtc(startTimeUtc, TimeZoneInfo.Local);

            long lTime = timeStamp * 10000000;
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        public static double GetTimestamp(DateTime time)
        {
            // 同样使用UTC基准时间
            DateTime startTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // UTC基准转本地基准
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeUtc, TimeZoneInfo.Local);

            return (time - startTime).TotalSeconds;
        }
    }
}
