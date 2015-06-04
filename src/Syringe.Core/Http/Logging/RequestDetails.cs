﻿using System;
using System.Collections.Generic;

namespace Syringe.Core.Http.Logging
{
	public class RequestDetails
	{
		public string Method { get; set; }
		public string Url { get; set; }
		public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
		public string Body { get; set; }
	}
}