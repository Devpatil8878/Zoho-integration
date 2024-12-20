﻿using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Zoho_integration.Models;
using System.Dynamic;
using System.Globalization;
using HtmlAgilityPack;
using System.Diagnostics;

public class ZohoController : Controller
{
    public bool WantRawRes = false;
    public List<object> RawSubTasks = new List<object>();
    async Task<(TaskContainer, object)> FetchRawTasks(Project project)
    {
        TaskContainer taskResponse = new TaskContainer();

        if (!(string.IsNullOrEmpty(project.Links.Task.Url)))
        {
            var tasks = await httpClient.GetAsync(project.Links.Task.Url);
            tasks.EnsureSuccessStatusCode();

            var tasksContent = await tasks.Content.ReadAsStringAsync();

            //return Ok(tasksContent);
            if (!(string.IsNullOrEmpty(tasksContent)))
            {
                taskResponse = JsonSerializer.Deserialize<TaskContainer>(tasksContent);
                var RawTask = JsonSerializer.Deserialize<object>(tasksContent);
                RawTasks.Add(RawTask);
            }

        }
        else
        {
            Console.WriteLine("Project: " + project.Name + " has no tasks");
        }

        return (taskResponse, RawTasks);
    }
    async Task<(List<User>, object)> FetchUsers(Project project)
    {
        List<User> users = new List<User>();
        object RawUsers = new object();

        try
        {
            var UsersResponse = await httpClient.GetAsync(project.Links.User.Url);
            UsersResponse.EnsureSuccessStatusCode();

            var UsersContent = await UsersResponse.Content.ReadAsStringAsync();
            RawUsers = JsonSerializer.Deserialize<object>(UsersContent);

            var userResponse = JsonSerializer.Deserialize<UserContainer>(UsersContent);

            foreach (var user in userResponse.Users)
                users.Add(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine("No users found");
        }

        return (users, RawUsers);
    }
    async Task<List<GComment>> FetchComments(Zoho_integration.Models.Task task)
    {
        var commentRes = await httpClient.GetAsync(task.Link.Self.Url + "comments/");
        var CommentResponseContent = await commentRes.Content.ReadAsStringAsync();
        var RawComment = new object();

        List<GComment> gComments = new List<GComment>();

        try
        {
            var comment = JsonSerializer.Deserialize<TaskCommentsResponse>(CommentResponseContent);
            RawComment = JsonSerializer.Deserialize<object>(CommentResponseContent);
            foreach (var com in comment.Comments)
            {
                var gComment = new GComment();
                var comAuthor = new GCommentAuthor();

                comAuthor.name = com.Author;
                comAuthor.email = "NA";
                comAuthor.id = "NA";

                gComment.id = com.Id;
                gComment.author = comAuthor;
                gComment.timestamp = !string.IsNullOrEmpty(com.Timestamp)
                                ? DateTime.ParseExact(com.Timestamp, "MM-dd-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                                    .ToString("yyyy-MM-ddTHH:mm:ss") : null;
                gComment.text = ExtractTextFromHtml(com.Text);

                gComments.Add(gComment);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("No comments found");
        }

        return gComments;
    }
    async Task<List<GTimeLog>> FetchTimeLogs(Zoho_integration.Models.Task task)
    {
        List<GTimeLog> gTimeLogs = new List<GTimeLog>();

        try
        {
            var timelogresponse = await httpClient.GetAsync(task.Link.Timesheet.Url);
            timelogresponse.EnsureSuccessStatusCode();

            var timelogstring = await timelogresponse.Content.ReadAsStringAsync();


            if (!string.IsNullOrEmpty(timelogstring))
            {
                var logcontainer = JsonSerializer.Deserialize<TimeLogContainer>(timelogstring);


                if (logcontainer != null && logcontainer.Timelogs?.TaskLogs != null)
                {
                    foreach (var log in logcontainer.Timelogs.TaskLogs)
                    {
                        Console.WriteLine(log);

                        LogUser logUser = new LogUser()
                        {
                            id = log.User.Id.ToString(),
                            name = log.User.Name,
                            email = "NA"
                        };

                        GTimeLog gTimeLog = new GTimeLog()
                        {
                            id = log.Id.ToString(),

                            DateLogged = !string.IsNullOrEmpty(log.DateLogged)
                                ? DateTime.ParseExact(log.DateLogged, "MM-dd-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                                    .ToString("yyyy-MM-ddTHH:mm:ss")
                                : null,

                            user = logUser,
                            timeSpent = log.timeSpent
                        };

                        gTimeLogs.Add(gTimeLog);
                    }
                }
                else
                {
                    Console.WriteLine("No task logs available.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("No timelogs found");
        }

        return gTimeLogs;
    }
    async Task<List<GSubtask>> FetchSubtasks(Zoho_integration.Models.Task task)
    {
        List<GSubtask> gSubtasks = new List<GSubtask>();
        try
        {
            var SubtaskResponse = await httpClient.GetAsync(task.Subtasks);
            //return Ok(task.Subtasks);
            SubtaskResponse.EnsureSuccessStatusCode();

            var SubtaskResponseContent = await SubtaskResponse.Content.ReadAsStringAsync();
            var subtaskResponse = JsonSerializer.Deserialize<SubTaskContainer>(SubtaskResponseContent);
            RawSubTasks.Add(JsonSerializer.Deserialize<object>(SubtaskResponseContent));

            var subtasks = subtaskResponse.Tasks;

            foreach (var subtask in subtasks)
            {
                gSubtasks.Add(new GSubtask
                {
                    id = subtask.Id.ToString(),
                    title = subtask.Name,
                    status = subtask.StatusName,
                    assignees = "NA"
                });
            }
        }
        catch
        {
            Console.WriteLine("No subtasks found");
        }

        return gSubtasks;
    }
    List<string> FetchTags(Zoho_integration.Models.Task task)
    {
        List<string> GTaskTags = new List<string>();
        try
        {
            foreach (var TaskTag in task?.Tags)
            {
                GTaskTags.Add(TaskTag.Name);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("No tags found");
        }

        return GTaskTags;
    }
    async Task<List<GTask>> FetchTasks(Project project, TaskContainer taskResponse)
    {
        List<GTask> gTasks = new List<GTask>();

        try
        {
            foreach (var task in taskResponse.Tasks)
            {

                List<GComment> gComments = await FetchComments(task);
                List<GTimeLog> gTimeLogs = await FetchTimeLogs(task);
                List<GSubtask> gSubtasks = await FetchSubtasks(task);
                var GTaskTags = FetchTags(task);

                //if ((!string.IsNullOrEmpty(task.Link.Document?.Url)))
                //{
                //    var docRes = await httpClient.GetAsync(task.Link.Document.Url);
                //    var docres = await docRes.Content.ReadAsStringAsync();
                //}

                Dictionary<string, string> CustomFieldsTemp = new Dictionary<string, string>();

                try
                {
                    foreach(var field in task.CustomFields)
                        CustomFieldsTemp.Add(field.fieldName, field.value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No custom fields found");
                }
                

                GTask gTask = new GTask
                {
                    id = task.Id.ToString(),
                    title = task.title,
                    description = ExtractTextFromHtml(task.Description),
                    status = task.StatusName,
                    priority = task.Priority,
                    assignees = "NA",
                    type = "NA",
                    tags = GTaskTags,
                    startDate = DateTime.ParseExact(task.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                    dueDate = DateTime.ParseExact(task.EndDate, "MM-dd-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                    timeEstimate = "NA",
                    timeSpent = "NA",
                    resolution = "NA",
                    reporter = task.Reporter,
                    customFields = CustomFieldsTemp,
                    subtasks = gSubtasks,
                    timelogs = gTimeLogs,
                    comments = gComments

                };

                gTasks.Add(gTask);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("No tasks found");
        }

        return gTasks;
    }
    public string ExtractTextFromHtml(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        if (string.IsNullOrEmpty(htmlContent))
        {
            return string.Empty;
        }
        doc.LoadHtml(htmlContent);
        return doc.DocumentNode.InnerText;
    }

    public static PortalResponse PortalResponse { get; set; }
    List<object> RawTasks = new List<object>();

    private string ClientId;
    private string ClientSecret;
    private const string RedirectUri = "http://localhost:5179/api/oauth/callback";
    private const string AuthUrl = "https://accounts.zoho.in/oauth/v2/auth";
    private const string TokenUrl = "https://accounts.zoho.in/oauth/v2/token";

    Stopwatch sw = new Stopwatch();

    public ZohoController(IConfiguration configuration)
    {
        //ClientId = Environment.GetEnvironmentVariable("Zoho_ClientId");
        //ClientSecret = Environment.GetEnvironmentVariable("Zoho_ClientSecret");
        ClientId = "1000.MM82IIGRW4W9CAZ3HH2HJ3WCAS2JUQ";
        ClientSecret = "4d41b0e5c62a44b2e7ae9741061da3ee705f95c5f9";
       
    }

    private static string accessToken;
    private static string refreshToken;
    private static DateTime accessTokenExpiryTime;
    HttpClient httpClient = new HttpClient();

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        var url = $"{AuthUrl}?response_type=code&client_id={ClientId}&scope=ZohoProjects.portals.READ ZohoProjects.portals.CREATE ZohoProjects.portals.UPDATE ZohoProjects.portals.DELETE ZohoProjects.projects.READ ZohoProjects.projects.CREATE ZohoProjects.projects.UPDATE ZohoProjects.projects.DELETE ZohoProjects.tasks.READ ZohoProjects.tasks.CREATE ZohoProjects.tasks.UPDATE ZohoProjects.tasks.DELETE ZohoProjects.milestones.READ ZohoProjects.milestones.CREATE ZohoProjects.milestones.UPDATE ZohoProjects.milestones.DELETE ZohoProjects.bugs.READ ZohoProjects.bugs.CREATE ZohoProjects.bugs.UPDATE ZohoProjects.bugs.DELETE ZohoProjects.forums.READ ZohoProjects.forums.CREATE ZohoProjects.forums.UPDATE ZohoProjects.forums.DELETE ZohoProjects.timesheets.READ ZohoProjects.timesheets.CREATE ZohoProjects.timesheets.UPDATE ZohoProjects.timesheets.DELETE ZohoProjects.users.READ ZohoProjects.users.CREATE ZohoProjects.users.UPDATE ZohoProjects.users.DELETE ZohoPC.files.ALL ZohoProjects.documents.ALL&redirect_uri={RedirectUri}&access_type=offline";
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

        if (Request.Cookies.TryGetValue("refreshToken", out string? refresh))
        {
            refreshToken = refresh;
            await RefreshAccessToken();
            Console.WriteLine("REFRESH TOKEN INVOKED");
        }
        else
        {
            await GetAccessToken(code);
            Console.WriteLine(refresh);
            Console.WriteLine("Access Token: " + accessToken);
            Console.WriteLine("Refresh Token: " + refreshToken);
            Console.WriteLine("Access Token Expiry Time: " + accessTokenExpiryTime);
        }

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
        try
        {
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
            accessToken = tokenResponse!.access_token;
            refreshToken = tokenResponse.refresh_token;
            accessTokenExpiryTime = DateTime.Now.AddSeconds(tokenResponse.expires_in);

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, 
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return tokenResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Token response is null.");

        }

        return null;
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
        sw.Start();
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

        var Projects = new List<Project>();
        dynamic? projectrawdata = new ExpandoObject();
        List<GProject> GResponse = new List<GProject>();
        List<GProjectSchema> gProjectSchemas = new List<GProjectSchema>();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken);

        try
        {
            var response = await httpClient.GetAsync("https://projectsapi.zoho.in/restapi/portals/");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            PortalResponse = JsonSerializer.Deserialize<PortalResponse>(data);
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
            return BadRequest("No portals found");
        }

        try
        {
            foreach (var portal in PortalResponse.portals)
            {
                var projectsRes = await httpClient.GetAsync(portal.Link.Project.Url);
                projectsRes.EnsureSuccessStatusCode();

                var projectsData = await projectsRes.Content.ReadAsStringAsync();
                projectrawdata = projectsData;

                var ProjectsResponse = JsonSerializer.Deserialize<Projects>(projectsData);
                foreach (var a in ProjectsResponse.ProjectList)
                    Projects.Add(a);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest("No portals found");
        }

        try
        {
            foreach (var project in Projects)
            {
                var Tasks = new Dictionary<string, Zoho_integration.Models.Task>();
                List<string> GTags = new List<string>();
                Dictionary<string, string> CustomFieldsTemp = new Dictionary<string, string>();

                var (taskResponse, RawTasks) = await FetchRawTasks(project);
                var (users, RawUsers) = await FetchUsers(project);
                List<GTask> gTasks = await FetchTasks(project, taskResponse);

                GOwner owner = new GOwner
                {
                    id = project.owner_id,
                    name = project.OwnerName,
                    email = project.owner_email,
                };

                if(project.Tags != null)
                    foreach (var tag in project.Tags)
                        GTags.Add(tag.Name);


                if (project.CustomFields != null)
                    foreach (var field in project.CustomFields)
                        CustomFieldsTemp.Add(field.fieldName, field.value);


                GProject gProject = new GProject
                {
                    Id = project.Id.ToString(),
                    Name = project.Name,
                    Description = ExtractTextFromHtml(project.Description),
                    StartDate = !string.IsNullOrEmpty(project.StartDate)
                                    ? DateTime.ParseExact(project.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture)
                                        .ToString("yyyy-MM-dd")
                                    : null
                    ,
                    EndDate = !string.IsNullOrEmpty(project.EndDate)
                                    ? DateTime.ParseExact(project.EndDate, "MM-dd-yyyy", CultureInfo.InvariantCulture)
                                        .ToString("yyyy-MM-dd")
                                    : null,
                    Tags = GTags,
                    CustomFields = "NA",
                    Status = project.Status,
                    Priority = "NA",
                    Owner = owner,
                    Tasks = gTasks,
                    resources = users

                };

                GResponse.Add(gProject);
                gProjectSchemas.Add(new GProjectSchema
                {
                    project = gProject,
                }) ;
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest("No projects found");
        }


        sw.Stop();
        Console.WriteLine("Time taken: " + sw.ElapsedMilliseconds);
            

        return Ok(gProjectSchemas);
        //return Ok(GResponse);
        //return Ok(projectrawdata);
        //return Ok(resp);
        //return Ok(RawSubTasks);
        //return Ok(users);
    }

}
