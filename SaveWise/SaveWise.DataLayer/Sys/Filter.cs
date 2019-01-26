using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SaveWise.DataLayer.Models;

namespace SaveWise.DataLayer.Sys
{
    public class Filter<T> where T : Document
    {
        private int _pageSize = 10;
        private int _pageIndex;

        public List<Expression<Func<T, bool>>> FilterExpressions { get; private set; } = new List<Expression<Func<T, bool>>>
        {
            entity => true
        };

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                {
                    value = 10;
                }
                
                _pageSize = value;
            }
        }

        public List<ColumnSort> Sorting { get; set; }

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (value <= 0)
                {
                    value = 0;
                }
                
                _pageIndex = value;
            }
        }

        public int PageNumber => PageIndex + 1;

        public void SetFilters(params Expression<Func<T, bool>>[] filterExpressions)
        {
            var filters = FilterExpressions;
            if (filterExpressions?.Any() == true)
            {
                filters = new List<Expression<Func<T, bool>>>();
                filters.AddRange(filterExpressions);
            }

            FilterExpressions = filters;
        }

        public Filter<T> AppendFilters(params Expression<Func<T, bool>>[] filterExpressions)
        {
            SetFilters(filterExpressions);
            return this;
        }
    }
}