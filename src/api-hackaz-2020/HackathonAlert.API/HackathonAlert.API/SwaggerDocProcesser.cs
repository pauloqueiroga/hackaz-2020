using System.Linq;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace HackathonAlert.API
{
    public class SwaggerDocProcesser : IDocumentProcessor
    {
        public void Process(DocumentProcessorContext context)
        {
            var document = context.Document;

            document.Info.Version = "v1";
            document.Info.Title = "Hackathon Alert API";
            document.Info.Description = "Simple API to 'get' and 'post' alerts for Hack-AZ";

            foreach (var pathsKey in document.Paths.Keys)
            {
                if (pathsKey.Equals("/api/Alert/{sourceIds}"))
                {
                    var pathItem = document.Paths[pathsKey];
                    // We know there is only one operation on this route, so just hardcoding it for now
                    var operationItem = pathItem.First().Value;

                    EditParamters(operationItem);
                }
            }
        }

        private static void EditParamters(OpenApiOperation operation)
        {
            foreach (var operationParameter in operation.Parameters)
            {
                if (operationParameter.Name.Equals("sourceIds"))
                {
                    operationParameter.Description =
                        "Comma separated string of Guid's given to each team to identity themselves.";
                }

                if (operationParameter.Name.Equals("minutesToSearch"))
                {
                    operationParameter.Description =
                        "Amount of minutes to go back in time and search for alerts";
                }
            }
        }
    }
}
