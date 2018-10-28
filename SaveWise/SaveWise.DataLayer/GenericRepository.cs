using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.DataLayer
{
    public class GenericRepository<TCollection> where TCollection : Document
    {
        private IMongoCollection<TCollection> Collection { get; set; }
        public string CollectionName { get; }
        
        public GenericRepository(IMongoCollection<TCollection> collection)
        {
            Collection = collection;
            CollectionName = collection.CollectionNamespace.CollectionName;
        }
        
        public virtual Task<List<TCollection>> Get<TFilter>(TFilter filter)
            where TFilter : Filter<TCollection>, new()
        {
            if (filter == null)
            {
                filter = new TFilter();
            }

            if (filter.FilterExpression == null)
            {
                filter.FilterExpression = collection => true;
            }

            var query = Collection.Find(filter.FilterExpression);

            query = query.Skip(filter.PageIndex * filter.PageSize);
            query = query.Limit(filter.PageSize);
            
            if (filter.Sorting?.Count > 0)
            {
                var builder = new SortDefinitionBuilder<TCollection>();
                foreach (var columnSort in filter.Sorting)
                {
                    var stringFieldDefinition = new StringFieldDefinition<TCollection>(columnSort.ColumnName);
                    Func<SortDefinition<TCollection>> sortDefinitionFunc;
                    if (columnSort.IsDescending)
                    {
                        sortDefinitionFunc = () => builder.Descending(stringFieldDefinition);
                    }
                    else
                    {
                        sortDefinitionFunc = () => builder.Ascending(stringFieldDefinition);
                    }

                    query = query.Sort(sortDefinitionFunc());
                }
            }

            return query.ToListAsync();
        }
        
        public virtual Task<TCollection> GetById(string id)
        {
            var query = Collection.Find(f => f.Id == id);
            return query.SingleOrDefaultAsync();
        }

        public virtual Task Insert(TCollection document)
        {
            return Collection.InsertOneAsync(document);
        }

        public virtual Task InsertMany(IEnumerable<TCollection> documents)
        {
            return Collection.InsertManyAsync(documents);
        }

        public virtual Task Update(string id, TCollection document)
        {
            return Collection.ReplaceOneAsync(f => string.Equals(f.Id, id), document);
        }

        public virtual async Task<bool> Delete(string id)
        {
            var actionResult = await Collection.DeleteOneAsync(f => f.Id == id);
            return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
        }

        public virtual async Task<bool> DeleteMany(IEnumerable<string> ids)
        {
            var result = await Collection.DeleteManyAsync(collection => ids.Contains(collection.Id));
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}