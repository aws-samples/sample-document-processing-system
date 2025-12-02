using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace DocumentProcessor.Web.Services;

public class SecretsService
{
    private readonly IAmazonSecretsManager _sm = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);

    public async Task<string> GetSecretAsync(string secretName)
    {
        try
        {
            var response = await _sm.GetSecretValueAsync(new GetSecretValueRequest { SecretId = secretName });
            return response.SecretString;
        }
        catch (Exception)
        {
            throw new Exception($"Secret not found: {secretName}");
        }
    }

    public async Task<string> GetSecretByDescriptionPrefixAsync(string prefix)
    {
        var list = await _sm.ListSecretsAsync(new ListSecretsRequest());
        foreach (var s in list.SecretList)
            if (!string.IsNullOrEmpty(s.Description) && s.Description.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return (await _sm.GetSecretValueAsync(new GetSecretValueRequest { SecretId = s.ARN })).SecretString;
        throw new Exception($"Secret not found: {prefix}");
    }

    public string GetFieldFromSecret(string secretJson, string fieldName)
    {
        using var doc = JsonDocument.Parse(secretJson);
        if (doc.RootElement.TryGetProperty(fieldName, out var val))
            return val.ValueKind == JsonValueKind.String ? val.GetString() ?? "" : val.ValueKind == JsonValueKind.Number ? val.GetInt32().ToString() : val.ToString();
        throw new Exception($"Field not found: {fieldName}");
    }
}
