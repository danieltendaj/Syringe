using System;

namespace Syringe.Web.Extensions
{
	public static class TimeSpanExtensions
	{
		public static string MinutesAndSecondsFormat(this TimeSpan timeSpan)
		{
			if (timeSpan.TotalSeconds < 1)
			{
				return $"{timeSpan.Milliseconds} milliseconds";
			}

			if (timeSpan.TotalMinutes < 1)
			{
				return $"{timeSpan.Seconds} seconds";
			}

			return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
		}
	}
}