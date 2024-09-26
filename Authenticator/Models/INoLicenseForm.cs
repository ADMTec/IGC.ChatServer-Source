using System;
using ToolsAuthServer.Models;

namespace Authenticator.Models
{
	public interface INoLicenseForm
	{
		ErrorCodes? AuthenticationError { get; set; }
	}
}
