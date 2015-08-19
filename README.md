# Force.com Toolkit for .NET [![Build Status](https://travis-ci.org/developerforce/Force.com-Toolkit-for-NET.svg?branch=master)](https://travis-ci.org/developerforce/Force.com-Toolkit-for-NET) [![Build status](https://ci.appveyor.com/api/projects/status/43y1npcb2q0u7aca)](https://ci.appveyor.com/project/WadeWegner/force-com-toolkit-for-net)

The Force.com Toolkit for .NET provide an easy way for .NET developers to interact with the Force.com, Force.com Bulk & Chatter REST APIs using native libraries.

These toolkits are built using the [Async/Await pattern](http://msdn.microsoft.com/en-us/library/hh191443.aspx) for asynchronous development and .NET [portable class libraries](http://msdn.microsoft.com/en-us/library/gg597391.aspx), making it easy to target multiple Microsoft platforms, including .NET 4.5, Windows Phone 8, Windows 8/8.1, and iOS/Android using Xamarin and Mono.NET.

The Common Libraries for .NET provides functionality used by the [Force.com Toolkit for .NET](https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/src/ForceToolkitForNET) and the [Chatter Toolkit for .NET](https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/src/ChatterToolkitForNET). While you can use the Common Libraries for .NET independently, it is recommended that you use it through one of the toolkits.

## NuGet Packages

### Published Packages

You can try the libraries immmediately by installing the following [DeveloperForce NuGet packages](https://www.nuget.org/profiles/DeveloperForce/).

```
Install-Package DeveloperForce.Force
Install-Package DeveloperForce.Chatter
```

## Samples

The toolkit includes the following sample applications.

### WebServerOAuthFlow

You can find this sample here: https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/samples/WebServerOAuthFlow

This sample shows how you can use the [Web Server OAuth Authentication Flow](http://www.salesforce.com/us/developer/docs/api_rest/Content/intro_understanding_web_server_oauth_flow.htm) to authorize a user and query the Force.com API. This sample uses MVC 5 and WebAPIs to demonstrate how you can retrieve a user access token and make an AJAX call to your API to retrieve data from Force.com and bind to your page using Knockout.js.

### Simple Console Application

You can find this sample here: https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/samples/SimpleConsole

This sample shows how to write a console application that leverages the async/await paradigm of .NET 4.5 and uses the toolkit to log in to and communicate with a Salesforce organization. Useful for quick POC applications and scheduled jobs.

### Simple Bulk Console Application

You can find this sample here: https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/samples/SimpleBulkConsole

This sample shows how to write a console application to create, update and delete multiple records using the bulk functionality in the toolkit.

### Advanced Bulk Console Application

You can find this sample here: https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/samples/AdvancedBulkConsole

This sample shows how to use the methods on the ```BulkForceClient``` to control bulk jobs step by step. It gives an example of a polling method that you could change to implement your own custom polling.

## Operations

Currently the following operations are supported.

### Authentication

To access the Force.com APIs you must have a valid Access Token. Currently there are two ways to generate an Access Token: the [Username-Password Authentication Flow](http://help.salesforce.com/HTViewHelpDoc?id=remoteaccess_oauth_username_password_flow.htm&language=en_US) and the [Web Server Authentication Flow](http://help.salesforce.com/apex/HTViewHelpDoc?id=remoteaccess_oauth_web_server_flow.htm&language=en_US)

#### Username-Password Authentication Flow

The Username-Password Authentication Flow is a straightforward way to get an access token. Simply provide your consumer key, consumer secret, username, and password.

```
var auth = new AuthenticationClient();

await auth.UsernamePasswordAsync("YOURCONSUMERKEY", "YOURCONSUMERSECRET", "YOURUSERNAME", "YOURPASSWORD");
```

#### Web-Server Authentication Flow

The Web-Server Authentication Flow requires a few additional steps but has the advantage of allowing you to authenticate your users and let them interact with the Force.com using their own access token.

First, you need to authetnicate your user. You can do this by creating a URL that directs the user to the Salesforce authentication service. You'll pass along some key information, including your consumer key (which identifies your Connected App) and a callback URL to your service.

```
var url =
    Common.FormatAuthUrl(
        "https://login.salesforce.com/services/oauth2/authorize", // if using test org then replace login with text
        ResponseTypes.Code,
        "YOURCONSUMERKEY",
        HttpUtility.UrlEncode("YOURCALLBACKURL"));
```

After the user logs in you'll need to handle the callback and retrieve the code that is returned. Using this code, you can then request an access token.

```
await auth.WebServerAsync("YOURCONSUMERKEY", "YOURCONSUMERSECRET", "YOURCALLBACKURL", code);
```

You can see a demonstration of this in the following sample application: https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/samples/WebServerOAuthFlow

#### Creating the ForceClient or BulkForceClient

After this completes successfully you will receive a valid Access Token and Instance URL. The Instance URL returned identifies the web service URL you'll use to call the Force.com REST APIs, passing in the Access Token. Additionally, the authentication client will return the API version number, which is used to construct a valid HTTP request.

Using this information, we can now construct our Force.com client.

````
var instanceUrl = auth.InstanceUrl;
var accessToken = auth.AccessToken;
var apiVersion = auth.ApiVersion;

var client = new ForceClient(instanceUrl, accessToken, apiVersion);
var bulkClient = new BulkForceClient(instanceUrl, accessToken, apiVersion);
```

### Sample Code

Below you'll find a few examples that show how to use the toolkit.

#### Create

You can create with the following code:

```
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

...

var account = new Account() { Name = "New Account", Description = "New Account Description" };
var id = await client.CreateAsync("Account", account);
```

You can also create with a non-strongly typed object:

```
var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
var account = new { Name = "New Name", Description = "New Description" };
var id = await client.CreateAsync("Account", account);
```

#### Update

You can update an object:

```
var account = new Account() { Name = "New Name", Description = "New Description" };
var id = await client.CreateAsync("Account", account);

account.Name = "New Name 2";

var success = await client.UpdateAsync("Account", id, account);
```

#### Delete

You can delete an object:

```
var account = new Account() { Name = "New Name", Description = "New Description" };
var id = await client.Create("Account", account);
var success = await client.DeleteAsync("Account", id)
```

#### Query

You can query for objects:


```
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

...

var accounts = await client.QueryAsync<Account>("SELECT id, name, description FROM Account");

foreach (var account in accounts.records)
{
    Console.WriteLine(account.Name);
}
```

### Bulk Sample Code

Below are some simple examples that show how to use the ```BulkForceClient```

**NOTE:** The following features are currently not supported

* CSV data type requests / responses
* Zipped attachment uploads
* Serial bulk jobs
* Query type bulk jobs

#### Create

You can create multiple records at once with the Bulk client:

```
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

...

var accountsBatch1 = new SObjectList<Account>
{
	new Account {Name = "TestStAccount1"},
	new Account {Name = "TestStAccount2"}
};
var accountsBatch2 = new SObjectList<Account>
{
	new Account {Name = "TestStAccount3"},
	new Account {Name = "TestStAccount4"}
};
var accountsBatch3 = new SObjectList<Account>
{
	new Account {Name = "TestStAccount5"},
	new Account {Name = "TestStAccount6"}
};

var accountsBatchList = new List<SObjectList<Account>> 
{ 
	accountsBatch1,
	accountsBatch2,
	accountsBatch3
};

var results = await bulkClient.RunJobAndPollAsync("Account", 
						Bulk.OperationType.Insert, accountsBatchList);
```

The above code will create 6 accounts in 3 batches. Each batch can hold upto 10,000 records and you can use multiple records.
For more details on the Salesforce Bulk API, see [the documentation](https://resources.docs.salesforce.com/196/latest/en-us/sfdc/pdf/api_asynch.pdf "Salesforce Bulk API Docs").

You can also create objects dynamically using the inbuilt SObject class:

```
var accountsBatch1 = new SObjectList<SObject>
{
	new SObject 
	{
		{"Name" = "TestDyAccount1"}
	},
	new SObject 
	{
		{"Name" = "TestDyAccount2"}
	}
};

var accountsBatchList = new List<SObjectList<SObject>> 
{ 
	accountsBatch1
};

var results = await bulkClient.RunJobAndPollAsync("Account", 
						Bulk.OperationType.Insert, accountsBatchList);

```
 
#### Update

Updating multiple records follows the same pattern as above, just change the ```Bulk.OperationType``` to ```Bulk.OperationType.Update```

```
var accountsBatch1 = new SObjectList<SObject>
{
	new SObject 
	{
		{"Id" = "YOUR_RECORD_ID"},
		{"Name" = "TestDyAccount1Renamed"}
	},
	new SObject 
	{
		{"Id" = "YOUR_RECORD_ID"},
		{"Name" = "TestDyAccount2Renamed"}
	}
};

var accountsBatchList = new List<SObjectList<SObject>> 
{ 
	accountsBatch1
};

var results = await bulkClient.RunJobAndPollAsync("Account", 
						Bulk.OperationType.Update, accountsBatchList);

```

#### Delete

As above, you can delete multiple records with ```Bulk.OperationType.Delete```

```
var accountsBatch1 = new SObjectList<SObject>
{
	new SObject 
	{
		{"Id" = "YOUR_RECORD_ID"}
	},
	new SObject 
	{
		{"Id" = "YOUR_RECORD_ID"}
	}
};

var accountsBatchList = new List<SObjectList<SObject>> 
{ 
	accountsBatch1
};

var results = await bulkClient.RunJobAndPollAsync("Account", 
						Bulk.OperationType.Delete, accountsBatchList);

```

## Contributing to the Repository ###

If you find any issues or opportunities for improving this respository, fix them!  Feel free to contribute to this project by [forking](http://help.github.com/fork-a-repo/) this repository and make changes to the content.  Once you've made your changes, share them back with the community by sending a pull request. Please see [How to send pull requests](http://help.github.com/send-pull-requests/) for more information about contributing to Github projects.

## Reporting Issues ###

If you find any issues with this demo that you can't fix, feel free to report them in the [issues](https://github.com/developerforce/Force.com-Toolkit-for-NET/issues) section of this repository.
