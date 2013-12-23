# Force.com Toolkit for .NET

*Note*: Please add all feedback - e.g. bugs, features, ideas, and so forth - as issues so that we can track them accordingly.

## Why a .NET Toolkit?

Why not? =)

More seriously, .NET developers represent a huge community and we want to make it as easy as possible for them to developer against Force.com. A toolkit, complete with libraries and NuGet packages, represents one of the easiest ways to get them started.

## NuGet Packages

### Published Packages

You can try the library immmediately by installing the this unlisted package.

```
Install-Package developerforce.restapi -Version 0.0.1
```

### DevTest Packages

If you want to use the most recent DevTest NuGet packages you can grab the packages built during the CI build. All you need to do is setup Visual Studio to pull from the build servers NuGet feed:

1. In Visual Studios select **Tools** -> **Library Package Manager** -> **Package Manager Settings**. Choose **Package Sources**.
2. Click **+** to add a new source.
3. Change the **Name** to "Force.com Toolkit for .NET (DevTest)".
4. Change the **Source** to http://dfbuild.cloudapp.net/guestAuth/app/nuget/v1/FeedService.svc/.

Now you can choose to install the latest NuGet from this DevTest feed.

## Sample Code

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
