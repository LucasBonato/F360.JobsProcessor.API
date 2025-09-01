using MongoDB.Driver;

namespace F360.JobsProcessor.API.Infrastructure;

public static class MongoDbExtensions {
	private static IMongoDatabase GetDatabase(this IMongoClient client)
		=> client.GetDatabase(AppEnv.DATABASE.MONGO.DATABASE_NAME.NotNull());

	private static IMongoCollection<Job> JobsCollection(this IMongoDatabase database)
		=> database.GetCollection<Job>(AppEnv.DATABASE.MONGO.TABLE.JOBS_COLLECTION_NAME.NotNull());
}