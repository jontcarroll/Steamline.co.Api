using System;
using Microsoft.AspNetCore.Mvc;

using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services;
using System.IO;

namespace Steamline.co.Api.V1.Helpers
{
    public static class ServiceActionResultFactory
    {

        public static ServiceActionResult Create<ErrorModel>(IServiceResult<ErrorModel> reuslt)
        {
            return new ServiceActionResult<ErrorModel>(reuslt);
        }

        public static ServiceActionResult Create<ErrorModel>(IFileServiceResult<ErrorModel> reuslt)
        {
            return new ServiceActionResult<ErrorModel>(reuslt);
        }

        public static ServiceActionResult Create<T, ErrorModel>(IServiceResult<T, ErrorModel> result)
        {
            return new ServiceActionResult<T, ErrorModel>(result);
        }

        private class ServiceActionResult<ErrorModel> : ServiceActionResult
        {
            public ServiceActionResult()
            {
                // Required or child classes can't override the constructor
            }

            public ServiceActionResult(IServiceResult<ErrorModel> serviceResult)
            {
                _actionResult = processResult(serviceResult);
            }

            public ServiceActionResult(IFileServiceResult<ErrorModel> serviceResult)
            {
                _actionResult = processResult(serviceResult);
            }

            public IActionResult processResult(IFileServiceResult<ErrorModel> result)
            {
                if (result.IsOk) {
                    return new FileStreamResult(result.Value, result.MimeType);
                }

                return processResult(result as IServiceResult<ErrorModel>);
            }

            protected IActionResult processResult(IServiceResult<ErrorModel> result)
            {
                if (result.IsOk)
                {
                    return new NoContentResult();
                }
                else if (result.NotFound)
                {
                    return new NotFoundResult();
                }
                else if (result.HasError)
                {
                    return new BadRequestObjectResult(result.Error);
                }
                else
                {
                    throw new Exception("Invalid state");
                }
            }
        }

        private class ServiceActionResult<T, ErrorModel> : ServiceActionResult<ErrorModel>
        {
            public ServiceActionResult(IServiceResult<T, ErrorModel> serviceResult)
            {
                _actionResult = processResult(serviceResult);
            }

            private IActionResult processResult(IServiceResult<T, ErrorModel> result)
            {
                if (result.IsOk)
                {
                    return new OkObjectResult(result.Value);
                }

                // Call processResult that only deals with base class to process the rest of the cases
                return processResult(result as IServiceResult<ErrorModel>);
            }
        }
    }
}