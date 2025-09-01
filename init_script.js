db = db.getSiblingDB('f360');

db.createCollection('jobs');
db.jobs.createIndex({ Status: 1 });
db.jobs.createIndex({ Type: 1 });
db.jobs.createIndex({ CreatedAt: 1 });