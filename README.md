# Force.com Toolkit for .NET [![Build Status](https://travis-ci.org/developerforce/Force.com-Toolkit-for-NET.svg?branch=master)](https://travis-ci.org/developerforce/Force.com-Toolkit-for-NET)

This SDK is now targeting .NET Standard 2.0, .NET 4.5.2, .NET 4.6.2, and .NET 4.7.2.

The Force.com Toolkit for .NET provides an easy way for .NET developers to interact with the Lighting Platform APIs using native libraries.

The Common Libraries for .NET provides functionality used by the [Force.com Toolkit for .NET](https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/src/ForceToolkitForNET) and the [Chatter Toolkit for .NET](https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/src/ChatterToolkitForNET). While you can use the Common Libraries for .NET independently, it is recommended that you use it through one of the toolkits.

## NuGet Packages

### Published Packages

You can try the libraries immmediately by installing the [DeveloperForce.Force](https://www.nuget.org/packages/DeveloperForce.Force/) and [DeveloperForce.Chatter](https://www.nuget.org/packages/DeveloperForce.Chatter/) packages:

Package Manager:

```
Install-Package DeveloperForce.Force
Install-Package DeveloperForce.Chatter
```

.NET CLI:

```
dotnet add package DeveloperForce.Force
dotnet add package DeveloperForce.Chatter
```

## Operations

Currently the following operations are supported.

### Authentication

To access the Force.com APIs you must have a valid Access Token. Currently there are two ways to generate an Access Token: the [Username-Password Authentication Flow](http://help.salesforce.com/HTViewHelpDoc?id=remoteaccess_oauth_username_password_flow.htm&language=en_US) and the [Web Server Authentication Flow](http://help.salesforce.com/apex/HTViewHelpDoc?id=remoteaccess_oauth_web_server_flow.htm&language=en_US)

#### Username-Password Authentication Flow

The Username-Password Authentication Flow is a straightforward way to get an access token. Simply provide your consumer key, consumer secret, username, and password concatenated with your API Token.

```cs
var auth = new AuthenticationClient();

await auth.UsernamePasswordAsync("YOURCONSUMERKEY", "YOURCONSUMERSECRET", "YOURUSERNAME",
                                 "YOURPASSWORDANDTOKEN");
```

#### Web-Server Authentication Flow

The Web-Server Authentication Flow requires a few additional steps but has the advantage of allowing you to authenticate your users and let them interact with the Force.com using their own access token.

First, you need to authenticate your user. You can do this by creating a URL that directs the user to the Salesforce authentication service. You'll pass along some key information, including your consumer key (which identifies your Connected App) and a callback URL to your service.

```cs
var url =
    Common.FormatAuthUrl(
        "https://login.salesforce.com/services/oauth2/authorize", // if using sandbox org then replace login with test
        ResponseTypes.Code,
        "YOURCONSUMERKEY",
        HttpUtility.UrlEncode("YOURCALLBACKURL"));
```

After the user logs in you'll need to handle the callback and retrieve the code that is returned. Using this code, you can then request an access token.

```cs
await auth.WebServerAsync("YOURCONSUMERKEY", "YOURCONSUMERSECRET", "YOURCALLBACKURL", code);
```

You can see a demonstration of this in the following sample application: https://github.com/developerforce/Force.com-Toolkit-for-NET/tree/master/samples/WebServerOAuthFlow

#### Creating the ForceClient

After this completes successfully you will receive a valid Access Token and Instance URL. The Instance URL returned identifies the web service URL you'll use to call the Force.com REST APIs, passing in the Access Token. Additionally, the authentication client will return the API version number, which is used to construct a valid HTTP request.

Using this information, we can now construct our Force.com client.

```cs
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

```cs
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

var account = new Account() { Name = "New Account", Description = "New Account Description" };
var id = await client.CreateAsync("Account", account);
```

You can also create with a non-strongly typed object:

```cs
var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
var account = new { Name = "New Name", Description = "New Description" };
var id = await client.CreateAsync("Account", account);
```

#### Update

You can update an object:

```cs
var account = new Account() { Name = "New Name", Description = "New Description" };
var id = await client.CreateAsync("Account", account);

account.Name = "New Name 2";

var success = await client.UpdateAsync("Account", id, account);
```

#### Delete

You can delete an object:

```cs
var account = new Account() { Name = "New Name", Description = "New Description" };
var id = await client.Create("Account", account);
var success = await client.DeleteAsync("Account", id)
```

#### Query

You can query for objects:

```cs
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}


var accounts = await client.QueryAsync<Account>("SELECT id, name, description FROM Account");

foreach (var account in accounts.records)
{
    Console.WriteLine(account.Name);
}
```

### Bulk Sample Code

Below are some simple examples that show how to use the `BulkForceClient`

**NOTE:** The following features are currently not supported

- CSV data type requests / responses
- Zipped attachment uploads
- Serial bulk jobs
- Query type bulk jobs

#### Create

You can create multiple records at once with the Bulk client:

```cs
public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

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
						BulkConstants.OperationType.Insert, accountsBatchList);
```

The above code will create 6 accounts in 3 batches. Each batch can hold upto 10,000 records and you can use multiple batches for Insert and all of the operations below.
For more details on the Salesforce Bulk API, see [the documentation](https://resources.docs.salesforce.com/196/latest/en-us/sfdc/pdf/api_asynch.pdf "Salesforce Bulk API Docs").

You can also create objects dynamically using the inbuilt SObject class:

```cs
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
                       BulkConstants.OperationType.Insert, accountsBatchList);
```

#### Update

Updating multiple records follows the same pattern as above, just change the `BulkConstants.OperationType` to `BulkConstants.OperationType.Update`

```cs
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
                       BulkConstants.OperationType.Update, accountsBatchList);
```

#### Delete

As above, you can delete multiple records with `BulkConstants.OperationType.Delete`

```cs
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
                       BulkConstants.OperationType.Delete, accountsBatchList);
```

#### Upsert

If your object includes a custom field with the External Id property set, you can use that to perform bulk upsert (update or insert) actions with `BulkConstants.OperationType.Upsert`. Note that you also have to specify the External Id field name when starting the job.

```cs
// Assumes you have a custom field "ExampleId" on your Account object
// that has the "External Id" flag set.

var accountsBatch1 = new SObjectList<SObject>
{
	new SObject
	{
		{"Name" = "TestDyAccount1"},
		{"ExampleId" = "ID00001"}
	},
	new SObject
	{
		{"Name" = "TestDyAccount2"},
		{"ExampleId" = "ID00002"}
	}
};

var accountsBatchList = new List<SObjectList<SObject>>
{
	accountsBatch1
};

var results = await bulkClient.RunJobAndPollAsync("Account", "ExampleId"
                       BulkConstants.OperationType.Upsert, accountsBatchList);

```

## Contributing to the Repository

If you find any issues or opportunities for improving this repository, fix them! Feel free to contribute to this project by [forking](http://help.github.com/fork-a-repo/) this repository and make changes to the content. Once you've made your changes, share them back with the community by sending a pull request. Please see [How to send pull requests](http://help.github.com/send-pull-requests/) for more information about contributing to Github projects.

## Reporting Issues

If you find any issues with this demo that you can't fix, feel free to report them in the [issues](https://github.com/developerforce/Force.com-Toolkit-for-NET/issues) section of this repository.
