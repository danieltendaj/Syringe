﻿namespace Syringe.Core
{
	public class HeaderItem
	{
		public string Key { get; set; }
		public string Value { get; set; }


		public HeaderItem(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public HeaderItem() : this("", "") { }
	}
}
