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

        public async Task Update(string id, T itemIn) =>
            await _items.ReplaceOneAsync(item => item.Id == id, itemIn);

        public async Task Remove(T itemIn) =>
            await _items.DeleteOneAsync(item => item.Id == itemIn.Id);

        public async Task Remove(string id) =>
            await _items.DeleteOneAsync(item => item.Id == id);

        protected virtual async Task<T> FindUpsert(T item)
        {
            return await Get(item.Id);
        }
        public async Task<T> Upsert(T item)
        {
            var r = await FindUpsert(item); 
            if (r == null)
                return await Insert(item);
            else
            {
                item.Id = r.Id;
                await Update(r.Id, item);
                return item;
            }
        }
    }

    public class MemuMongoBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
