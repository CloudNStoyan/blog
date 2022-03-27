using System;

namespace Blog.Web.Infrastructure
{
    public static class DateUtils
    {
        public static string DateTimeToLongAgo(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }

            var now = DateTime.Now;

            var difference = now - dateTime.Value;

            if (difference.Days > 365)
            {
                return $"{Math.Floor(difference.Days / 365d)} years ago";
            }

            if (difference.Days > 30)
            {
                return $"{Math.Floor(difference.Days / 30d)}";
            }

            if (difference.Days > 0)
            {
                return $"{difference.Days} days ago";
            }

            if (difference.Hours > 0)
            {
                return $"{difference.Hours} hours ago";
            }

            if (difference.Minutes > 0)
            {
                return $"{difference.Minutes} minutes ago";
            }

            if (difference.Seconds > 0)
            {
                return $"{difference.Seconds} seconds ago";
            }

            return "just now";
        }
    }
}
