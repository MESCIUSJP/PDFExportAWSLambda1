using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PDFExportAWSLambda1
{
    public class Function
    {

        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            APIGatewayProxyResponse response;

            string queryString;
            input.QueryStringParameters.TryGetValue("name", out queryString);

            string Message = string.IsNullOrEmpty(queryString)
                ? "Hello, World!!"
                : $"Hello, {queryString}!!";

            //GcPdfDocument.SetLicenseKey("");

            GcPdfDocument doc = new GcPdfDocument();
            GcPdfGraphics g = doc.NewPage().Graphics;

            g.DrawString(Message,
                new TextFormat() { Font = StandardFonts.Helvetica, FontSize = 12 },
                new PointF(72, 72));

            var base64String = "";

            using (var ms = new MemoryStream())
            {
                doc.Save(ms, false);
                base64String = Convert.ToBase64String(ms.ToArray());
            }

            response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = base64String,
                IsBase64Encoded = true,
                Headers = new Dictionary<string, string> {
                    { "Content-Type", "application/pdf" },
                    { "Content-Disposition", "attachment; filename=Result.pdf"},
                }
            };

            return response;
        }
    }
}
