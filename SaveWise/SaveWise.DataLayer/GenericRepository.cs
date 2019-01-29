using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using SaveWise.DataLayer.Sys;
using Document = SaveWise.DataLayer.Models.Document;

namespace SaveWise.DataLayer
{
    public class GenericRepository<TCollection> : IGenericRepository<TCollection> where TCollection : Document
    {
        private readonly IIdentityProvider _identityProvider;
        private IMongoCollection<TCollection> Collection { get; set; }
        public string CollectionName => Collection?.CollectionNamespace.CollectionName;

        public GenericRepository(
            ISaveWiseContext context,
            IIdentityProvider identityProvider,
            string collectionName)
        {
            _identityProvider = identityProvider;
            Collection = context.Database.GetCollection<TCollection>(collectionName);
        }

        public virtual Task<List<TCollection>> GetAsync<TFilter>(TFilter filter = null)
            where TFilter : Filter<TCollection>, new()
        {
            return GetAsAsync(item => item, filter);
        }
        
        public virtual Task<List<TNew>> GetAsAsync<TFilter, TNew>(
            Expression<Func<TCollection, TNew>> selectClause,
            TFilter filter = null)
            where TFilter : Filter<TCollection>, new()
        {
            if (filter == null)
            {
                filter = new TFilter();
            }
            
            IFindFluent<TCollection, TCollection> query = Collection.Find(BuildFilterDefinition(filter));

            if (filter.Sorting?.Count > 0)
            {
                var builder = new SortDefinitionBuilder<TCollection>();
                foreach (ColumnSort columnSort in filter.Sorting)
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
            
            query = query.Skip(filter.PageIndex * filter.PageSize);
            query = query.Limit(filter.PageSize);

            return query.Project(selectClause).ToListAsync();
        }

        public virtual Task<TCollection> GetByIdAsync(string id)
        {
            IFindFluent<TCollection, TCollection> query = Collection
                .Find(BuildFilterDefinition(f => string.Equals(f.Id, id)));
            return query.SingleOrDefaultAsync();
        }

        public virtual TCollection GetById(string id)
        {
            IFindFluent<TCollection, TCollection> query = Collection
                .Find(BuildFilterDefinition(f => string.Equals(f.Id, id)));
            return query.SingleOrDefault();
        }

        public virtual Task InsertAsync(TCollection document)
        {
            document.UserId = _identityProvider.GetUserId();
            return Collection.InsertOneAsync(document);
        }

        public virtual Task InsertManyAsync(IEnumerable<TCollection> documents)
        {
            string userId = _identityProvider.GetUserId();

            IList<TCollection> collection = documents.ToList();
            Parallel.ForEach(collection, document => { document.UserId = userId; });

            return Collection.InsertManyAsync(collection);
        }

        public virtual Task UpdateAsync(string id, TCollection document)
        {
            document.UserId = _identityProvider.GetUserId();
            return Collection.ReplaceOneAsync(BuildFilterDefinition(f => string.Equals(f.Id, id)), document);
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            DeleteResult actionResult = await Collection.DeleteOneAsync(BuildFilterDefinition(f => string.Equals(f.Id, id)));
            return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
        }

        public virtual async Task<bool> DeleteManyAsync(IEnumerable<string> ids)
        {
            DeleteResult result = await Collection.DeleteManyAsync(BuildFilterDefinition(collection => ids.Contains(collection.Id)));
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        private FilterDefinition<TCollection> BuildFilterDefinition<TFilter>(TFilter filter)
            where TFilter : Filter<TCollection>, new()
        {
            var filters = filter.FilterExpressions ?? new List<Expression<Func<TCollection, bool>>>();
            if (filters.Count == 0)
            {
                filters.Add(t => true);
            }

            filters.Add(collection => string.Equals(collection.UserId, _identityProvider.GetUserId()));
            
            return new FilterDefinitionBuilder<TCollection>()
                .And(filters.Select(t=>new ExpressionFilterDefinition<TCollection>(t)));
        }

        private FilterDefinition<TCollection> BuildFilterDefinition(Expression<Func<TCollection, bool>> expression)
        {
            return new FilterDefinitionBuilder<TCollection>().And(
                expression,
                new ExpressionFilterDefinition<TCollection>(collection =>
                    string.Equals(collection.UserId, _identityProvider.GetUserId())));
        }
    }
}