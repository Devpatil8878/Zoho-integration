using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class TokenResponse
{
    public string access_token { get; set; }  // Note: change to lowercase
    public string refresh_token { get; set; }  // Note: change to lowercase
    public string expires_in { get; set; }
    public string token_type { get; set; }
    public string api_domain { get; set; }
    public string refresh_token_expires_in { get; set; }
}


public class ZohoController : Controller
{
    private const string ClientId = "1000.MR1HJSCB129FTS3BWJJIMY13O9LRVL";
    private const string ClientSecret = "840f7a65af25b855f12a6c1ed9f794b96b42a8f22d";
    private const string RedirectUri = "http://localhost:5179/api/oauth/callback";
    private const string AuthUrl = "https://accounts.zoho.in/oauth/v2/auth";
    private const string TokenUrl = "https://accounts.zoho.in/oauth/v2/token";

    private static string accessToken;
    private static string refreshToken;
    private static string scope = "ZohoProjects.portals.READ ZohoProjects.portals.CREATE ZohoProjects.portals.UPDATE ZohoProjects.portals.DELETE ZohoProjects.projects.READ ZohoProjects.projects.CREATE ZohoProjects.projects.UPDATE ZohoProjects.projects.DELETE ZohoProjects.tasks.READ ZohoProjects.tasks.CREATE ZohoProjects.tasks.UPDATE ZohoProjects.tasks.DELETE ZohoProjects.milestones.READ ZohoProjects.milestones.CREATE ZohoProjects.milestones.UPDATE ZohoProjects.milestones.DELETE ZohoProjects.bugs.READ ZohoProjects.bugs.CREATE ZohoProjects.bugs.UPDATE ZohoProjects.bugs.DELETE ZohoProjects.forums.READ ZohoProjects.forums.CREATE ZohoProjects.forums.UPDATE ZohoProjects.forums.DELETE ZohoProjects.timesheets.READ ZohoProjects.timesheets.CREATE ZohoProjects.timesheets.UPDATE ZohoProjects.timesheets.DELETE ZohoProjects.users.READ ZohoProjects.users.CREATE ZohoProjects.users.UPDATE ZohoProjects.users.DELETE";

    // Step 1: Redirect user to Zoho for authorization
    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        var url = $"{AuthUrl}?response_type=code&client_id={ClientId}&scope=ZohoProjects.portals.READ ZohoProjects.portals.CREATE ZohoProjects.portals.UPDATE ZohoProjects.portals.DELETE ZohoProjects.projects.READ ZohoProjects.projects.CREATE ZohoProjects.projects.UPDATE ZohoProjects.projects.DELETE ZohoProjects.tasks.READ ZohoProjects.tasks.CREATE ZohoProjects.tasks.UPDATE ZohoProjects.tasks.DELETE ZohoProjects.milestones.READ ZohoProjects.milestones.CREATE ZohoProjects.milestones.UPDATE ZohoProjects.milestones.DELETE ZohoProjects.bugs.READ ZohoProjects.bugs.CREATE ZohoProjects.bugs.UPDATE ZohoProjects.bugs.DELETE ZohoProjects.forums.READ ZohoProjects.forums.CREATE ZohoProjects.forums.UPDATE ZohoProjects.forums.DELETE ZohoProjects.timesheets.READ ZohoProjects.timesheets.CREATE ZohoProjects.timesheets.UPDATE ZohoProjects.timesheets.DELETE ZohoProjects.users.READ ZohoProjects.users.CREATE ZohoProjects.users.UPDATE ZohoProjects.users.DELETE&redirect_uri={RedirectUri}&access_type=offline";
        return Redirect(url);
    }

    // Step 2: Handle the redirect back from Zoho
    [HttpGet("oauth/callback")]
    public async Task<IActionResult> Callback(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Console.WriteLine("Authorization code is null or empty.");
            return BadRequest("Authorization code is null or empty.");
        }

        Console.WriteLine("Received authorization code: " + code);

        var tokenResponse = await GetAccessToken(code);

        if (tokenResponse == null)
        {
            Console.WriteLine("Token response is null.");
            return BadRequest("Failed to retrieve tokens.");
        }

        // Store the access token and refresh token in variables
        accessToken = tokenResponse.access_token; // Ensure property names match the response
        refreshToken = tokenResponse.refresh_token;

        Console.WriteLine("Access Token: " + accessToken);
        Console.WriteLine("Refresh Token: " + refreshToken);

        return Ok(new { accessToken, refreshToken });
    }


    // Method to exchange authorization code for access token
    private async Task<TokenResponse> GetAccessToken(string code)
    {
        using var httpClient = new HttpClient();
        var values = new Dictionary<string, string>
    {
        { "grant_type", "authorization_code" },
        { "client_id", ClientId },
        { "client_secret", ClientSecret },
        { "redirect_uri", RedirectUri },
        { "code", code }
    };

        var content = new FormUrlEncodedContent(values);
        var response = await httpClient.PostAsync(TokenUrl, content);

        // Log the raw response
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Raw Token Response: " + responseBody);

        // Check for success
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode} - {responseBody}");
            return null;
        }

        // Deserialize the response to TokenResponse model
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

        return tokenResponse;
    }



    // Method to fetch data from Zoho Projects
    [HttpGet("fetch-data")]
    public async Task<IActionResult> FetchData()
    {
        // Use the stored access token variable
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);

        var response = await httpClient.GetAsync("https://projectsapi.zoho.in/restapi/portal/0112it211017gmaildotcom/projects/");
        //response.EnsureSuccessStatusCode();
        Console.WriteLine(accessToken);
        Console.WriteLine(refreshToken);
        Console.WriteLine();
        Console.WriteLine("Response: " + response);
        var data = await response.Content.ReadAsStringAsync();
        return Ok(data);
    }
}

