using System.IO;

namespace Steamline.co.Api.V1.Services.Interfaces
{
    public interface IServiceResult<ErrorModel>
    {
        bool IsOk { get; }
        bool HasError { get; }
        bool NotFound { get; }

        ErrorModel Error { get; }
    }

    public interface IServiceResult<T, ErrorModel> : IServiceResult<ErrorModel>
    {
        T Value { get; }
    }

    public interface IFileServiceResult<ErrorModel> : IServiceResult<ErrorModel>
    {
        Stream Value { get; }
        string MimeType { get; }
    }
}