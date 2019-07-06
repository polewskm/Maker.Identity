using System;

namespace Maker.Identity.Stores
{
	public static class Constants
	{
		// 9999-12-31 00:00:00.000
		public static readonly DateTimeOffset MaxDateTimeOffset = new DateTimeOffset(9999, 12, 31, 0, 0, 0, TimeSpan.Zero);
	}
}