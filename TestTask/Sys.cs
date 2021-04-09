using System;

namespace TestTask
{
   public class Sys
    {
        private DateTime _sunrise;
        private DateTime _sunset;
        public string Sunrise
        {
            get
            {
                return $"{_sunrise.Hour}:{_sunrise.Minute}";
            }
            set
            {
                if (long.TryParse(value, out long time) )
                {
                    _sunrise = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;       
                }

            }
        }
       
        public string Sunset
        {
            get
            {
                return $"{_sunset.Hour}:{_sunset.Minute}";
            }
            set
            {
                if (long.TryParse(value, out long time))
                {
                    _sunset = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
                }
            }
        }
    }
}
