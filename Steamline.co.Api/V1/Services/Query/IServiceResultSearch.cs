using System.Collections.Generic;

using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services;

using Steamline.co.Api.V1.Models;

namespace Steamline.co.Api.V1.Services.Query
{
    public interface IServiceResultSearch<T> : IServiceResultPager<T>
    {
        IServiceResultPager<T> Search(string q, Dictionary<string, string> orderBy = null);
    }
}