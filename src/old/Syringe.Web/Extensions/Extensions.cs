using System;

namespace Syringe.Web.Extensions
{
	public static class Extensions
	{
		public static string MinutesAndSecondsFormat(this TimeSpan timeSpan)
		{
			if (timeSpan.TotalSeconds < 1)
			{
				return string.Format("{0} milliseconds", timeSpan.Milliseconds);
			}

			if (timeSpan.TotalMinutes < 1)
			{
				return string.Format("{0} seconds", timeSpan.Seconds);
			}

			return string.Format("{0}m {1}s", timeSpan.Minutes, timeSpan.Seconds);
		}
	}
}