using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using Syringe.Core.Configuration;

namespace Syringe.Core.Environment.Json
{
	public class JsonEnvironmentProvider : IEnvironmentProvider
	{
		private readonly IOptions<Environments> _options;
		private Environment[] _environments;

		public JsonEnvironmentProvider(IOptions<Environments> options)
		{
			_options = options;
		}

		public IEnumerable<Environment> GetAll()
		{
			if (_environments == null)
			{
				if (_options.Value != null && _options.Value.Any())
				{
					_environments = _options.Value.OrderBy(x => x.Order).ToArray();
				}
				else
				{
					_environments = new Environment[0];
				}
			}

			return _environments;
		}
	}
}