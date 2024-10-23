using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using Zoho_integration.Models;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;

public class TokenResponse
{
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public int expires_in { get; set; }
    public string token_type { get; set; }
    public string api_domain { get; set; }
    public int refresh_token_expires_in { get; set; }
}

public class ZohoController : Controller
{

    public static Project Projects { get; set; }
    public static Zoho_integration.Models.Task TasksList { get; set; }
    public static Comment CommentsList { get; set; }
    public static Attachment AttachmentList { get; set; }
    public static User UserList { get; set; }
    public static Subtask SubtaskList { get; set; }



    public static PortalResponse PortalResponse { get; set; }




    private const string ClientId = "1000.2XR72MGYVJBIGMZMRAQI2HF6QBJSSU";
    private const string ClientSecret = "fce2ac89994471b1287443b6368504172aec4e8dde";
    private const string RedirectUri = "http://localhost:5179/api/oauth/callback";
    private const string AuthUrl = "https://accounts.zoho.in/oauth/v2/auth";
    private const string TokenUrl = "https://accounts.zoho.in/oauth/v2/token";

    private static string accessToken;
    private static string refreshToken;
    private static DateTime accessTokenExpiryTime;

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        var url = $"{AuthUrl}?response_type=code&client_id={ClientId}&scope=ZohoProjects.portals.READ ZohoProjects.portals.CREATE ZohoProjects.portals.UPDATE ZohoProjects.portals.DELETE ZohoProjects.projects.READ ZohoProjects.projects.CREATE ZohoProjects.projects.UPDATE ZohoProjects.projects.DELETE ZohoProjects.tasks.READ ZohoProjects.tasks.CREATE ZohoProjects.tasks.UPDATE ZohoProjects.tasks.DELETE ZohoProjects.milestones.READ ZohoProjects.milestones.CREATE ZohoProjects.milestones.UPDATE ZohoProjects.milestones.DELETE ZohoProjects.bugs.READ ZohoProjects.bugs.CREATE ZohoProjects.bugs.UPDATE ZohoProjects.bugs.DELETE ZohoProjects.forums.READ ZohoProjects.forums.CREATE ZohoProjects.forums.UPDATE ZohoProjects.forums.DELETE ZohoProjects.timesheets.READ ZohoProjects.timesheets.CREATE ZohoProjects.timesheets.UPDATE ZohoProjects.timesheets.DELETE ZohoProjects.users.READ ZohoProjects.users.CREATE ZohoProjects.users.UPDATE ZohoProjects.users.DELETE ZohoProjects.documents.ALL ZohoPC.files.READ&redirect_uri={RedirectUri}&access_type=offline";
        return Redirect(url);
    }

    [HttpGet("api/oauth/callback")]
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

        accessToken = tokenResponse.access_token;
        refreshToken = tokenResponse.refresh_token;

        accessTokenExpiryTime = DateTime.Now.AddSeconds(tokenResponse.expires_in);

        Console.WriteLine("Access Token: " + accessToken);
        Console.WriteLine("Refresh Token: " + refreshToken);
        Console.WriteLine("Access Token Expiry Time: " + accessTokenExpiryTime);

        //return Ok();
        return RedirectToAction("FetchData");
    }

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
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Raw Token Response: " + responseBody);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode} - {responseBody}");
            return null;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return tokenResponse;
    }

    private async Task<TokenResponse> RefreshAccessToken()
    {
        using var httpClient = new HttpClient();
        var values = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", ClientId },
            { "client_secret", ClientSecret },
            { "refresh_token", refreshToken }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await httpClient.PostAsync(TokenUrl, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Raw Refresh Token Response: " + responseBody);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode} - {responseBody}");
            return null;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

        if (tokenResponse != null)
        {
            accessToken = tokenResponse.access_token;
            accessTokenExpiryTime = DateTime.Now.AddSeconds(tokenResponse.expires_in);
        }

        return tokenResponse;
    }

    [HttpGet("fetch-data")]
    public async Task<IActionResult> FetchData()
    {
        if (DateTime.Now >= accessTokenExpiryTime)
        {
            Console.WriteLine("Access token has expired, refreshing...");
            var tokenResponse = await RefreshAccessToken();

            if (tokenResponse == null)
            {
                return BadRequest("Failed to refresh access token.");
            }

            Console.WriteLine("New Access Token: " + accessToken);
        }

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);

        var response = await httpClient.GetAsync("https://projectsapi.zoho.in/restapi/portals/");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();

        var portalResponse = JsonSerializer.Deserialize<PortalResponse>(data);
        Console.WriteLine("Fetched Data: " + data);
        PortalResponse = portalResponse;

        var Projects = new List<Project>();

        dynamic? projectrawdata = new ExpandoObject();

        Projects AllProjects = new Zoho_integration.Models.Projects();

        foreach(var portal in PortalResponse.portals)
        {
            var projectsRes = await httpClient.GetAsync(portal.Link.Project.Url);
            response.EnsureSuccessStatusCode();


            var projectsData = await projectsRes.Content.ReadAsStringAsync();
            projectrawdata = projectsData;
            var ProjectsResponse = JsonSerializer.Deserialize<Projects>(projectsData);
            foreach(var a in ProjectsResponse.ProjectList)
            {
                Projects.Add(a);
            }
            AllProjects = ProjectsResponse;

            Console.WriteLine("Fetched Data: " + ProjectsResponse);
            PortalResponse = portalResponse;


        }




       





        var Tasks = new Dictionary<string, Zoho_integration.Models.Task>();

        List<GProject> GResponse = new List<GProject>();

        List<string> dddd = new List<string>();

        List<object> temp = new List<object>();
        

        foreach(var project in Projects)
        {
            var tasks = await httpClient.GetAsync(project.Links.Task.Url);
            tasks.EnsureSuccessStatusCode();

            var dd = await tasks.Content.ReadAsStringAsync();
            var taskResponse = JsonSerializer.Deserialize<TaskContainer>(dd);
            temp.Add(tasks);

            var res = await httpClient.GetAsync(project.Links.User.Url);
            res.EnsureSuccessStatusCode();

            var d = await res.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<UserContainer>(d);

            var users = new List<User>();

            foreach (var user in userResponse.Users)
            {
                users.Add(user);

            }

            List<GTask> gTasks = new List<GTask>();

            dynamic ddddd = new ExpandoObject();
            List<GTimeLog> gTimeLogs = new List<GTimeLog>();

            foreach (var task in taskResponse.Tasks)
            {
                Tasks[project.Name] = task;
                List<GSubtask> gSubtasks = new List<GSubtask>();

                if (task.Link.Timesheet.Url != null)
                {

                    var timelogresponse = await httpClient.GetAsync(task.Link.Timesheet.Url);
                    timelogresponse.EnsureSuccessStatusCode();

                    var timelogstring = await timelogresponse.Content.ReadAsStringAsync();

                    var logcontainer = JsonSerializer.Deserialize<TimelogContainer>(timelogstring);

                    

                    foreach (var log in logcontainer.Timelogs.Tasklogs)
                    {
                        GTimeLog gTimeLog = new GTimeLog()
                        {
                            id = log.Id.ToString(),
                            DateLogged = DateTime.ParseExact(log.DateLogged, "MM-dd-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture).ToString("yyyy-MM-ddTHH:mm:ss"),
                            user = log.User,
                            timeSpent = log.timeSpent
                        };

                        gTimeLogs.Add(gTimeLog);
                    }

                }

                if (task.Subtasks != null)
                {
                    var sb = await httpClient.GetAsync(task.Subtasks);
                    sb.EnsureSuccessStatusCode();

                    var sub = await sb.Content.ReadAsStringAsync();
                    var subtaskResponse = JsonSerializer.Deserialize<SubTaskContainer>(sub);
                    var subtaskss = subtaskResponse.Tasks;

                    foreach (var subtask in subtaskss)
                    {
                        gSubtasks.Add(new GSubtask
                        {
                            id = subtask.Id.ToString(),
                            title = subtask.Name,
                            status = subtask.StatusName,
                        });
                    }

                    
                }

                


                GTask gTask = new GTask
                {
                    id = task.Id.ToString(),
                    title = task.title,
                    description = task.Description,
                    status = task.StatusName,
                    priority = task.Priority,
                    startDate = DateTime.ParseExact(task.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                    dueDate = DateTime.ParseExact(task.EndDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                    Subtasks = gSubtasks,
                    timelogs = gTimeLogs

                };

                gTasks.Add(gTask);


            }

            GOwner owner = new GOwner
            {
                id = project.owner_id,
                name = project.OwnerName,
                email = project.owner_email,
            };

           

            GProject gProject = new GProject
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Priority = project.Priority,
                Owner = owner,
                Tasks = gTasks,
                resources = users

            };

            GResponse.Add(gProject);
        }






        








        //return Ok(projectrawdata);
        //return Ok(rrrr);
        return Ok(GResponse);
        //return Ok(Tasks);
        //return Ok(users);
        //return Ok(temp);
    }
}
