using System.IO;
using Steamline.co.Api.V1.Services.Interfaces;

namespace Steamline.co.Api.V1.Services
{
    public static class ServiceResultFactory
    {
        public static IServiceResult<ErrorModel> Ok<ErrorModel>()
        {
            var result = new ServiceResult<ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.Ok    
            };

            return result;
        }

        public static IServiceResult<T, ErrorModel> Ok<T, ErrorModel>(T value)
        {
            var result = new ServiceResult<T, ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.Ok,
                Value = value
            };

            return result;
        }

        public static IFileServiceResult<ErrorModel> Ok<ErrorModel>(Stream file, string mimeType)
        {
            var result = new FileServiceResult<ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.Ok,
                Value = file,
                MimeType = mimeType
            };

            return result;
        }

        public static IServiceResult<ErrorModel> Error<ErrorModel>(ErrorModel model)
        {
            var result = new ServiceResult<ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.Error,
                Error = model
            };

            return result;
        }

        public static IServiceResult<T, ErrorModel> Error<T, ErrorModel>(ErrorModel model)
        {
            var result = new ServiceResult<T, ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.Error,
                Error = model
            };

            return result;
        }

        public static IFileServiceResult<ErrorModel> FileError<ErrorModel>(ErrorModel model)
        {
            var result = new FileServiceResult<ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.Error,
                Error = model
            };

            return result;
        }

        public static IServiceResult<ErrorModel> NotFound<ErrorModel>()
        {
            var result = new ServiceResult<ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.NotFound
            };

            return result;
        }

        public static IServiceResult<T, ErrorModel> NotFound<T, ErrorModel>()
        {
            var result = new ServiceResult<T, ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.NotFound
            };

            return result;
        }

        public static IFileServiceResult<ErrorModel> FileNotFound<ErrorModel>()
        {
            var result = new FileServiceResult<ErrorModel>() {
                State = ServiceResult<ErrorModel>.States.NotFound
            };

            return result;
        }

        private class ServiceResult<ErrorModel> : IServiceResult<ErrorModel>
        {
            public enum States
            {
                Ok,
                NotFound,
                Error
            };

            private States _state;
            public States State
            {
                set
                {
                    _state = value;
                }
            }

            public bool IsOk
            {
                get
                {
                    return _state == States.Ok;
                }
            }

            public bool NotFound
            {
                get
                {
                    return _state == States.NotFound;
                }
            }

            public bool HasError
            {
                get
                {
                    return _state == States.Error;
                }
            }

            public ErrorModel Error { get; set; }
        }

        private class ServiceResult<T, ErrorModel> : ServiceResult<ErrorModel>, IServiceResult<T, ErrorModel>
        {
            public T Value { get; set; }
        }

        private class FileServiceResult<ErrorModel> : ServiceResult<ErrorModel>, IFileServiceResult<ErrorModel>
        {
            public Stream Value { get; set; }
            public string MimeType { get; set; }
        }
    }
}