using System;

namespace ToolsAuthServer.Models
{
	public enum ErrorCodes : byte
	{
		NONE,
		PRIVATE_Unknown,
		PRIVATE_InvalidToolID = 100,
		PRIVATE_InvalidPacket_Server,
		PRIVATE_InvalidPacket_Client,
		PRIVATE_InvalidHandshake_Server,
		PRIVATE_InvalidSystemInfo,
		PRIVATE_PacketMaxLenReached_Server,
		PRIVATE_PacketMaxLenReached_Client,
		PRIVATE_InvalidHandshake_Client,
		PUBLIC_LicenseNotFound = 200,
		PUBLIC_ServerUnreachable,
		PUBLIC_ServcerConnectionLost,
		PUBLIC_CryptFail
	}
}
