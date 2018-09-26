using System.Collections.Generic;

using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services;

using Steamline.co.Api.V1.Models;

namespace Steamline.co.Api.V1.Services.Query
{
    public interface IServiceResultPager<T>
    {
        IServiceResult<Page<T>, ApiErrorModel> Page(Dictionary<string, int> paging);
    }
}