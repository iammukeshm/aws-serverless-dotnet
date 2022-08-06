using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StudentLambda;

public class Function
{

    public async Task<List<Student>> GetAllStudentsAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        DynamoDBContext dbContext = new DynamoDBContext(client);
        var data = await dbContext.ScanAsync<Student>(default).GetRemainingAsync();
        return data;
    }
    public async Task<APIGatewayHttpApiV2ProxyResponse> CreateStudentAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var studentRequest = JsonConvert.DeserializeObject<Student>(request.Body);
        AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        DynamoDBContext dbContext = new DynamoDBContext(client);
        await dbContext.SaveAsync(studentRequest);
        var message = $"Student with Id {studentRequest?.Id} Created";
        LambdaLogger.Log(message);
        return new APIGatewayHttpApiV2ProxyResponse
        {
            Body = message,
            StatusCode = 200
        };
    }
    public async Task<Student> GetStudentByIdAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        DynamoDBContext dbContext = new DynamoDBContext(client);
        string idFromPath = request.PathParameters["id"];
        int id = Int32.Parse(idFromPath);
        var student = await dbContext.LoadAsync<Student>(id);
        if (student == null) throw new Exception("Not Found!");
        return student;
    }
}
