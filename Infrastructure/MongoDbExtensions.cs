using F360.JobsProcessor.API.Domain;
using MongoDB.Driver;

namespace F360.JobsProcessor.API.Infrastructure;

public static class MongoDbExtensions {
	public static IMongoCollection<Job> JobsCollection(this IMongoDatabase database)
		=> database.GetCollection<Job>(AppEnv.DATABASE.MONGO.TABLE.JOBS_COLLECTION_NAME.NotNull());
}