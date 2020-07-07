using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemuMongoBase
{
    public class MemuMongoBaseService<T, T2> where T : MemuMongoBase
    {

        protected readonly IMongoCollection<T> _items;
        private readonly IMapper _mapper;

        public MemuMongoBaseService(IMapper mapper, String connectionString, String databaseName, String collectionName)
        {
            _mapper = mapper;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _items = database.GetCollection<T>(collectionName);

        }

        protected virtual T2 Convert(T obj)
        {
            return _mapper.Map<T2>(obj);
        }
        protected virtual List<T2> ConvertList(List<T> objs)
        {
            return _mapper.Map<List<T>, List<T2>>(objs);
        }

        public virtual async Task<List<T2>> Get()
        {
            var items = await _items.Find(race => true).ToListAsync();
            return ConvertList(items);
        }

        public async Task<T2> Get(string id) =>
            Convert(await _items.Find<T>(item => item.Id == id).FirstOrDefaultAsync());
        public virtual async Task<List<T>> GetRaw()
        {
            var items = await _items.Find(race => true).ToListAsync();
            return items;
        }

        public async Task<T> GetRaw(string id) =>
            await _items.Find<T>(item => item.Id == id).FirstOrDefaultAsync();

        public async Task<T> InsertRaw(T item)
        {
            await _items.InsertOneAsync(item);
            return item;
        }
    }

    public class MemuMongoBaseService<T> where T : MemuMongoBase
    {

        protected readonly IMongoCollection<T> _items;
        
        public MemuMongoBaseService(String connectionString, String databaseName, String collectionName)
        {
            
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _items = database.GetCollection<T>(collectionName);

        }

        public virtual async Task<List<T>> Get()
        {
            var items = await _items.Find(race => true).ToListAsync();
            return items;
        }

        public async Task<T> Get(string id) =>
            await _items.Find<T>(item => item.Id == id).FirstOrDefaultAsync();                

        public async Task<T> Insert(T item)
        {
            await _items.InsertOneAsync(item);
            return item;
        }
    }

    public class MemuMongoBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
