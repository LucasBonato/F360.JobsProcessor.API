using Anv;

namespace F360.JobsProcessor.API;

public static class AppEnv {
	public static class MESSAGE_BROKER {
		public static readonly AnvEnv HOST = new("MESSAGE_BROKER__HOST");
		public static readonly AnvEnv USERNAME = new("MESSAGE_BROKER__USERNAME");
		public static readonly AnvEnv PASSWORD = new("MESSAGE_BROKER__PASSWORD");
	}

	public static class DATABASE {
		public static class MONGO {
			public static readonly AnvEnv CONNECTION_URI = new("DATABASE__MONGO__CONNECTION_URI");
			public static readonly AnvEnv DATABASE_NAME = new("DATABASE__MONGO__DATABASE_NAME");

			public static class TABLE {
				public static readonly AnvEnv JOBS_COLLECTION_NAME = new("DATABASE__MONGO__TABLE__JOBS_COLLECTION_NAME");
			}
		}
	}
}