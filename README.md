# Force.com Toolkit for .NET <img src="http://dfbuild.cloudapp.net/app/rest/builds/buildType:ForceComToolkitForNet_DebugCiBuild/statusIcon" />

The Force.com Toolkit for .NET provides an easy way for .NET developers to interact with the Force.com REST API using a native libraries. This toolkit is built using the [Async/Await pattern](http://msdn.microsoft.com/en-us/library/hh191443.aspx) for asynchronous development and .NET [portable class libraries](http://msdn.microsoft.com/en-us/library/gg597391.aspx), making it easy to target multiple Microsoft platforms, including .NET 4/4.5, Windows Phone8, Windows 8/8.1, and Silverlight 5.

## NuGet Packages

### Published Packages

You can try the library immmediately by installing this [NuGet package](http://www.nuget.org/packages/DeveloperForce.Force/).

```
Install-Package DeveloperForce.Force
```

### DevTest Packages

If you want to use the most recent DevTest NuGet packages you can grab the packages built during the CI build. All you need to do is setup Visual Studio to pull from the build servers NuGet feed:

1. In Visual Studios select **Tools** -> **Library Package Manager** -> **Package Manager Settings**. Choose **Package Sources**.
2. Click **+** to add a new source.
3. Change the **Name** to "Force.com Toolkit for .NET (DevTest)".
4. Change the **Source** to http://dfbuild.cloudapp.net/guestAuth/app/nuget/v1/FeedService.svc/.

Now you can choose to install the latest NuGet from this DevTest feed.

## Samples

The toolkit includes the following sample applications.

### WebServerOAuthFlow

You can fine this sample here: https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/samples/WebServerOAuthFlow

This sample shows how you can use the [Web Server OAuth Authentication Flow](http://www.salesforce.com/us/developer/docs/api_rest/Content/intro_understanding_web_server_oauth_flow.htm) to authorize a user and query the Force.com API. This sample uses MVC 5 and WebAPIs to demonstrate how you can retrieve a user access token and make an AJAX call to your API to retrieve data from Force.com and bind to your page using Knockout.js.

You can see this demo live here: https://sfdcauth.cloudapp.net/

### SimpleConsole



## Operations

Currently the following operations are supported.

### Authenticate

I'd recommend getting the values out of a App.Config or Web.Config file:

```
private static string _securityToken = ConfigurationSettings.AppSettings["SecurityToken"];
private static string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
private static string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
private static string _username = ConfigurationSettings.AppSettings["Username"];
private static string _password = ConfigurationSettings.AppSettings["Password"] + _securityToken;
```

You can call *Authenticate* directly or you can have it implicitly called in the constructor by passing in the appropriate values:

````
var client = new ForceClient();
await client.Authenticate(_consumerKey, _consumerSecret, _username, _password);
```

... or ...

```
var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
```

### Create

You can create with the following code:

```
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

...

var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
var account = new Account() {Name = "New Name", Description = "New Description"};
var id = await client.Create("Account", account);
```

You can also create with a non-strongly typed object:

```
var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
var account = new { Name = "New Name", Description = "New Description" };
var id = await client.Create("Account", account);
```

### Update

You can update an object:

```
var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
var account = new Account() { Name = "New Name", Description = "New Description" };
var id = await client.Create("Account", account);

account.Name = "New Name 2";

var success = await client.Update("Account", id, account);
```

### Delete

You can delete an object:

```
var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);

var account = new Account() { Name = "New Name", Description = "New Description" };
var id = await client.Create("Account", account);
var success = await client.Delete("Account", id)
```

### Query

You can query for objects:


```
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

...

var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");
```
