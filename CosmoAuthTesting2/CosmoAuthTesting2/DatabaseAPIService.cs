using System.Net;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;


namespace CosmoAuthTesting2
{
    internal class DatabaseAPIService
    {
        static readonly string endpoint = "UriWithPortAndTrailingSlash";
        static readonly string masterKey = "MASTERKEY";
        static readonly Uri baseUri = new Uri(endpoint);

        static readonly string databaseId = "";
        static readonly string collectionId = "";
        static readonly string documentId = "";
        
        static readonly string utc_date = DateTime.UtcNow.ToString("r");

        private HttpClient client;
        private string token;

        public DatabaseAPIService()
        {
            client = new HttpClient();
            token = new InteractiveBrowserCredential().GetToken(new TokenRequestContext(["UriNoPort/.default"])).Token;
        }

        public void GetDocument()
        {
            string resourceLink = string.Format("dbs/{0}/colls/{1}/docs/{2}", databaseId, collectionId, documentId);

            //string authHeader = GenerateAuthorizationSignature(token);
            string authHeader = GenerateMasterKeyAuthorizationSignature(HttpMethod.Get, "docs", resourceLink, utc_date, masterKey);


            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", utc_date);
            client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            client.DefaultRequestHeaders.Add("Authorization", authHeader);
            client.DefaultRequestHeaders.Add("x-ms-documentdb-partitionkey", "[\"/id\"]");
            client.DefaultRequestHeaders.Add("s-ms-consistency-level", "Eventual");

            try 
            {
                var uri = new Uri(baseUri, resourceLink);
                Console.WriteLine(uri);
                Console.WriteLine(client.DefaultRequestHeaders.ToString());
                string response = client.GetStringAsync(uri).Result;
                Console.WriteLine(response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.ReadKey();
        }

        public void ListDocuments()
        {
            //Microsoft Docs here are wrong, use the location of the resource being accessed, not the location of the endpoint
            string resourceId = string.Format("dbs/{0}/colls/{1}", databaseId, collectionId);
            string resourceLink = resourceId + "/docs";

            //string authHeader = GenerateMasterKeyAuthorizationSignature(HttpMethod.Get, "docs", resourceId, utc_date, masterKey);

            string authHeader = GenerateAuthorizationSignature(token);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", utc_date);
            client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            client.DefaultRequestHeaders.Add("Authorization", authHeader);

            try
            {
                var uri = new Uri(baseUri, resourceLink);
                Console.WriteLine(uri);
                Console.WriteLine(client.DefaultRequestHeaders.ToString());
                string response = client.GetStringAsync(uri).Result;
                Console.WriteLine(response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        public void ListDatabases()
        {
            string resourceLink = $"";

            string authHeader = GenerateMasterKeyAuthorizationSignature(HttpMethod.Get, ResourceType.dbs.ToString(), resourceLink, utc_date, masterKey);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", utc_date);
            client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            client.DefaultRequestHeaders.Add("authorization", authHeader);

            try
            {
                var uri = new Uri(baseUri, resourceLink);
                Console.WriteLine(uri);
                Console.WriteLine(client.DefaultRequestHeaders.ToString());
                string response = client.GetStringAsync(uri).Result;
                Console.WriteLine(response);
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GetCollection()
        {
            string resourceLink = string.Format("dbs/{0}/colls/{1}", databaseId, collectionId);

            string authHeader = GenerateMasterKeyAuthorizationSignature(HttpMethod.Get, ResourceType.colls.ToString(), resourceLink, utc_date, masterKey);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", utc_date);
            client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            client.DefaultRequestHeaders.Add("Authorization", authHeader);

            try
            {
                var uri = new Uri(baseUri, resourceLink);
                Console.WriteLine(uri);
                Console.WriteLine(client.DefaultRequestHeaders.ToString());
                string response = client.GetStringAsync(uri).Result;
                Console.WriteLine(response);
                Console.ReadKey();

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GetDatabase()
        {
            string resourceLink = string.Format("dbs/{0}", databaseId);

            string authHeader = GenerateMasterKeyAuthorizationSignature(HttpMethod.Get, ResourceType.dbs.ToString(), resourceLink, utc_date, masterKey);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", utc_date);
            client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            client.DefaultRequestHeaders.Add("Authorization", authHeader);

            try
            {
                var uri = new Uri(baseUri, resourceLink);
                Console.WriteLine(uri);
                Console.WriteLine(client.DefaultRequestHeaders.ToString());
                string response = client.GetStringAsync(uri).Result;
                Console.WriteLine(response);
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string GenerateAuthorizationSignature(string token)
        {
            var keyType = "aad";
            var tokenVersion = "1.0";
            
            var authSet = WebUtility.UrlEncode($"type={keyType}&ver={tokenVersion}&sig={token}");

            return authSet;
        }

        private static string GenerateMasterKeyAuthorizationSignature(HttpMethod verb, string resourceType, string resourceLink, string date, string key)
        {
            var keyType = "master";
            var tokenVersion = "1.0";
            var payload = $"{verb.ToString().ToLowerInvariant()}\n{resourceType.ToLowerInvariant()}\n{resourceLink}\n{date.ToLowerInvariant()}\n\n";

            var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };
            var hashPayload = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
            var signature = Convert.ToBase64String(hashPayload);
            var authSet = WebUtility.UrlEncode($"type={keyType}&ver={tokenVersion}&sig={signature}");

            return authSet;
        }
        public enum ResourceType
        {
            dbs,
            colls,
            docs,
            sprocs,
            pkranges,
        }
    }
    
}
