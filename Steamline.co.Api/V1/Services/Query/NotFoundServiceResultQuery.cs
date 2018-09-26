using System;
using System.Collections.Generic;
using System.Linq;


using Steamline.co.Api.V1.Models;
using Steamline.co.Api.V1.Services.Utils;
using Steamline.co.Api.V1.Services.Interfaces;

namespace Steamline.co.Api.V1.Services.Query
{
    public class NotFoundServiceResultQuery<T> : IServiceResultFilter<T>
    {
        public NotFoundServiceResultQuery() {

        }

        public IServiceResultOrderBy<T> Filter(Dictionary<string, string> filter)
        {
            return this;
        }

        public IServiceResultSearch<T> OrderBy(Dictionary<string, string> orderBy)
        {
            return this;
        }

        public IServiceResult<Page<T>, ApiErrorModel> Page(Dictionary<string, int> paging)
        {
            return ServiceResultFactory.NotFound<Page<T>, ApiErrorModel>();
        }

        public IServiceResultPager<T> Search(string q, Dictionary<string, string> orderBy = null)
        {
            return this;
        }
    }
}