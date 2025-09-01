using MongoDB.Driver;

namespace F360.JobsProcessor.API.Infrastructure;

public static class MongoDbExtensions {

	public static IServiceCollection AddMongoDb(this IServiceCollection services) {
		return services
			.AddSingleton<IMongoClient>(_ => new MongoClient(AppEnv.DATABASE.MONGO.CONNECTION_URI.NotNull()))
			.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(AppEnv.DATABASE.MONGO.DATABASE_NAME.NotNull()))
			;
	}

	public static IMongoCollection<Job> JobsCollection(this IMongoDatabase database)
		=> database.GetCollection<Job>(AppEnv.DATABASE.MONGO.TABLE.JOBS_COLLECTION_NAME.NotNull());
}