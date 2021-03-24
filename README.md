# Storage Triggers

Here we will examine some alternative mechanisms for initiating a cloud workflow in Azure. In some cases it will be obvious what the initiating event is and other times it will not be and factors such as cost, user experience, development and maintenance time, etc. will all need to be factored in to find the correct solution. When using Azure Functions or Azure Logic Apps it is possible to use a mechanism called a trigger to initiate a call to the function/LA. One type of trigger that can be utilised is known as a Blob Trigger. When a file is dropped into Azure Blob Storage or any of an existing Blob's properties are changed the trigger will be activated.

However, it is important to note that this mechanism comes with some caveats: From the docs [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-trigger?tabs=csharp
)

> "If you require faster or more reliable blob processing, consider creating a queue message when you create the blob. Then use a queue trigger instead of a blob trigger to process the blob. Another option is to use Event Grid; see the tutorial Automate resizing uploaded images using Event Grid."

![warning image](./images/warning.png)

Each sample provides a mechanism to respond to a file being uploaded to Azure Blob Storage. The response will be to place a message onto a Azure Service bus instance and thus potentially trigger a workflow of some kind but that is left as an exercise for the reader:

## Blob Trigger

The BlobTrigger sample uses a standard BlobTrigger to start a Logic App. It is tempting just to reach for this but these triggers can take a long time to fire and that may not even happen at all. You can read more details about about how the BlobTrigger works [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-trigger?tabs=csharp).

In order to try this out we will create a Logic App that uses a BlobTrigger. The Logic App itself, once started will post a message to an Azure Service Bus and we could know that it is working as expected by viewing messages on the Service Bus Queue.

So, in the Azure Portal we can create our Logic App:

![Create Logic App](./images/create-logic-app.png)

Also, we are going to need a storage account:

> We will use this storage account throughout and also we will place all resources into the same resource group.

![Create Storage Account](./images/create-storage-account.png)



If you would like to deploy the Azure resources to your own subscription please click the following link the ensure you fill in the parameters according to their descriptions.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fpeted70%2Fstorage-triggers%2Fmain%2Fblob-trigger%2Ftemplate%2Ftemplate.json%3Ftoken%3DAAONK2LMWTLJ7HJJDP3QI5LALDFYW" target="_blank">
    <img src="https://aka.ms/deploytoazurebutton"/>
</a>

## Poll Blob

The polling version uses a recurrence trigger that fires on a schedule. When the trigger fires the Logic app will enumerate a storage container and check whether a specific property is set on a Blob's metadata.

> There currently isn't a Blob Storage connector to retrieve the metadata so we have an Azure Function that does this job and is added to the Logic flow using an Azure Function connector.

The polling Logic app will then send a service bus message if the custom metadata value has not been previously set otherwise no other action will be taken.

## Event Grid

## Send Message