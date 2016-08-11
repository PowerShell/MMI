/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Globalization;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Datetime
    {
        [FieldOffset(0)]
        internal bool isTimestamp;

        [FieldOffset(4)]
        internal MI_Timestamp timestamp;

        [FieldOffset(4)]
        internal MI_Interval interval;

        internal static MI_Datetime ConvertToDateTime(object value)
        {
            if (value is TimeSpan)
            {
                return new MI_Datetime((TimeSpan)value);
            }
            else if (value is DateTime)
            {
                return new MI_Datetime((DateTime)value);
            }
            else
            {
                return new MI_Datetime((DateTime)Convert.ToDateTime(value, CultureInfo.InvariantCulture));
            }
        }

        internal object ConvertFromNativeLayer()
        {
            if (this.isTimestamp)
            {
                // "Now" value defined in line 1934, page 53 of DSP0004, version 2.6.0
                if ((this.timestamp.year == 0) &&
                    (this.timestamp.month == 1) &&
                    (this.timestamp.day == 1) &&
                    (this.timestamp.hour == 0) &&
                    (this.timestamp.minute == 0) &&
                    (this.timestamp.second == 0) &&
                    (this.timestamp.microseconds == 0) &&
                    (this.timestamp.utc == 720))
                {
                    return DateTime.Now;
                }
                // "Infinite past" value defined in line 1935, page 54 of DSP0004, version 2.6.0
                else if ((this.timestamp.year == 0) &&
                    (this.timestamp.month == 1) &&
                    (this.timestamp.day == 1) &&
                    (this.timestamp.hour == 0) &&
                    (this.timestamp.minute == 0) &&
                    (this.timestamp.second == 0) &&
                    (this.timestamp.microseconds == 999999) &&
                    (this.timestamp.utc == 720))
                {
                    return DateTime.MinValue;
                }
                // "Infinite future" value defined in line 1936, page 54 of DSP0004, version 2.6.0
                else if ((this.timestamp.year == 9999) &&
                    (this.timestamp.month == 12) &&
                    (this.timestamp.day == 31) &&
                    (this.timestamp.hour == 11) &&
                    (this.timestamp.minute == 59) &&
                    (this.timestamp.second == 59) &&
                    (this.timestamp.microseconds == 999999) &&
                    (this.timestamp.utc == (-720)))
                {
                    return DateTime.MaxValue;
                }
                else
                {
#if !_CORECLR
                    DateTime managedUtcDateTime = new DateTime(
                                             (int)this.timestamp.year,
                                             (int)this.timestamp.month,
                                             (int)this.timestamp.day,
                                             (int)this.timestamp.hour,
                                             (int)this.timestamp.minute,
                                             (int)this.timestamp.second,
                                             (int)this.timestamp.microseconds / 1000,
                                             CultureInfo.InvariantCulture.Calendar,
                                             DateTimeKind.Utc);
#else
                    Calendar myCalendar = CultureInfo.InvariantCulture.Calendar;
                    DateTime managedDateTime = myCalendar.ToDateTime(
                                             (int)this.timestamp.year,
                                             (int)this.timestamp.month,
                                             (int)this.timestamp.day,
                                             (int)this.timestamp.hour,
                                             (int)this.timestamp.minute,
                                             (int)this.timestamp.second,
                                             (int)this.timestamp.microseconds / 1000);
                    DateTime managedUtcDateTime = DateTime.SpecifyKind(managedDateTime, DateTimeKind.Utc);

#endif
                    long microsecondsUnaccounted = this.timestamp.microseconds % 1000;
                    managedUtcDateTime = managedUtcDateTime.AddTicks(microsecondsUnaccounted * 10); // since 1 microsecond == 10 ticks
                    managedUtcDateTime = managedUtcDateTime.AddMinutes(-(this.timestamp.utc));


#if !_CORECLR
                    DateTime managedLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(managedUtcDateTime, TimeZoneInfo.Local);
#else
                    //
                    // TODO: USE THIS FOR BOTH CORECLR AND FULLOS
                    //
                    DateTime managedLocalDateTime = TimeZoneInfo.ConvertTime(managedUtcDateTime, TimeZoneInfo.Local);
#endif

                    return managedLocalDateTime;
                }
            }
            else
            {
                if (TimeSpan.MaxValue.TotalDays < this.interval.days)
                {
                    return TimeSpan.MaxValue;
                }

                try
                {
                    TimeSpan managedTimeSpan = new TimeSpan(
                                             (int)this.interval.days,
                                             (int)this.interval.hours,
                                             (int)this.interval.minutes,
                                             (int)this.interval.seconds,
                                             (int)this.interval.microseconds / 1000);
                    long microsecondsUnaccounted = this.interval.microseconds % 1000;
                    TimeSpan ticksUnaccountedTimeSpan = new TimeSpan(microsecondsUnaccounted * 10); // since 1 microsecond == 10 ticks

                    return managedTimeSpan.Add(ticksUnaccountedTimeSpan);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return TimeSpan.MaxValue;
                }
            }
            
        }

        internal MI_Datetime(TimeSpan interval)
        {
            this.timestamp = new MI_Timestamp();
            this.interval = new MI_Interval();

            if (interval.Equals(TimeSpan.MaxValue))
            {
                this.interval.days = 99999999;
                this.interval.hours = 23;
                this.interval.minutes = 59;
                this.interval.seconds = 59;
                this.interval.microseconds = 0;
            }
            else
            {
                long ticksUnaccounted = interval.Ticks % 10000; // since 10000 ticks == 1 millisecond
                this.interval.days = (uint)interval.Days;
                this.interval.hours = (uint)interval.Hours;
                this.interval.minutes = (uint)interval.Minutes;
                this.interval.seconds = (uint)interval.Seconds;
                this.interval.microseconds = (uint)(interval.Milliseconds * 1000 + ticksUnaccounted / 10);
            }
            this.isTimestamp = false;
        }

        static DateTime maxValidCimTimestamp = new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);

        internal MI_Datetime(DateTime datetime)
        {
            this.timestamp = new MI_Timestamp();
            this.interval = new MI_Interval();

            if (datetime.Equals(DateTime.MaxValue))
            {
                // "Infinite future" value defined in line 1936, page 54 of DSP0004, version 2.6.0
                this.timestamp.year = 9999;
                this.timestamp.month = 12;
                this.timestamp.day = 31;
                this.timestamp.hour = 11;
                this.timestamp.minute = 59;
                this.timestamp.second = 59;
                this.timestamp.microseconds = 999999;
                this.timestamp.utc = (-720);
            }
            else if (datetime.Equals(DateTime.MinValue))
            {
                // "Infinite past" value defined in line 1935, page 54 of DSP0004, version 2.6.0
                this.timestamp.year = 0;
                this.timestamp.month = 1;
                this.timestamp.day = 1;
                this.timestamp.hour = 0;
                this.timestamp.minute = 0;
                this.timestamp.second = 0;
                this.timestamp.microseconds = 999999;
                this.timestamp.utc = 720;
            }
            else if (DateTime.Compare(maxValidCimTimestamp, datetime) <= 0)
            {
                // "Youngest useable timestamp" value defined in line 1930, page 53 of DSP0004, version 2.6.0
                this.timestamp.year = 9999;
                this.timestamp.month = 12;
                this.timestamp.day = 31;
                this.timestamp.hour = 11;
                this.timestamp.minute = 59;
                this.timestamp.second = 59;
                this.timestamp.microseconds = 999998;
                this.timestamp.utc = (-720);
            }
            else
            {

                datetime = TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.Utc);
                long ticksUnaccounted = datetime.Ticks % 10000; // since 10000 ticks == 1 millisecond

                this.timestamp.year = (uint)datetime.Year;
                this.timestamp.month = (uint)datetime.Month;
                this.timestamp.day = (uint)datetime.Day;
                this.timestamp.hour = (uint)datetime.Hour;
                this.timestamp.minute = (uint)datetime.Minute;
                this.timestamp.second = (uint)datetime.Second;
                this.timestamp.microseconds = (uint)(datetime.Millisecond * 1000 + ticksUnaccounted / 10); // since 1 tick == 0.1 microsecond
                this.timestamp.utc = 0;
            }

            this.isTimestamp = true;
        }
    }
}
