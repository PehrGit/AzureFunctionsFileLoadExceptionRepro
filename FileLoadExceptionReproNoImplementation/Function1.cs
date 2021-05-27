using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FileLoadExceptionRepro
{
    public static class Function1
    {
        [FunctionName("Function1")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DummyListModel), Example = typeof(DummyListModelExample), Required = true, Description = "Dummy list model")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(DummyStringModel), Example = typeof(DummyStringModelExample), Summary = "Dummy response", Description = "This returns the dummy response")]

        public static async Task<IActionResult> UpdateDummies(
            [HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "dummies")] HttpRequest req,
            ILogger log)
        {
            var content = new List<DummyStringModel>();
            var result = new OkObjectResult(content);

            return await Task.FromResult(result).ConfigureAwait(false);
        }
    }

    [OpenApiExample(typeof(DummyStringModelExample))]
    public class DummyStringModel
    {
        public string StringValue { get; set; }

        public Uri UriValue { get; set; }
    }

    public class DummyStringModelExample : OpenApiExample<DummyStringModel>
    {
        public override IOpenApiExample<DummyStringModel> Build(NamingStrategy namingStrategy = null)
        {
            this.Examples.Add(OpenApiExampleResolver.Resolve("lorem", new DummyStringModel() { StringValue = "Hello World", UriValue = new Uri("http://localhost:80") }, namingStrategy));
            this.Examples.Add(OpenApiExampleResolver.Resolve("ipsum", new DummyStringModel() { StringValue = "Hello World 2", UriValue = new Uri("https://localhost:443") }, namingStrategy));

            return this;
        }
    }

    public class DummyListModel
    {
        public List<DummyStringModel> ListValues { get; set; }
    }

    public class DummyListModelExample : OpenApiExample<DummyListModel>
    {
        public override IOpenApiExample<DummyListModel> Build(NamingStrategy namingStrategy = null)
        {
            this.Examples.Add(
                OpenApiExampleResolver.Resolve(
                    "dummy",
                    new DummyListModel()
                    {
                        ListValues = new List<DummyStringModel>() { new DummyStringModel() { StringValue = "Hello World", UriValue = new Uri("https://localhost") } }
                    },
                    namingStrategy
                ));

            return this;
        }
    }
}

