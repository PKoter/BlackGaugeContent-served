namespace Bgc.Api
{
	public class R
	{
		public const int MaxGendersCount  = 255;
		public const int MemesOnPageCount = 10;

		public struct Privileges
		{
			public const string User  = "BgcUser";
			public const string Mod   = "BgcMod";
			public const string Admin = "BgcGod";
		}

		public struct AuthTags
		{
			public const string Role      = "rol";
			public const string Id        = "id";
			public const string ApiAccess = "api_access";
		}

		public struct ModelRules
		{
			public const int MinNameLength  = 5;
			public const int MaxNameLength  = 128;

			public const int MinEmailLength = 4;
			public const int MaxEmailLength = 256;

			public const int MaxMessageLength = 2048;
		}

		public struct ImpulseType
		{
			public const string ComradeRequest = "comradeRequest";
			public const string Message        = "message";
		}
	}
}
