# Azure Service Bus & Event Hub Simulator

Today there are so many questions about Azure Cloud Solution Architecture Ques products, Custome Caching, Advanced Throttling & Performance.
## Ques Products:
1. Service Bus Q.
2. Service Bus Topics & Subscriptions.
3. Event Hub & Stream Analytics.
4. APIM.

![alt tag](https://raw.githubusercontent.com/zivshtaeinberg/AzureServicebusAndEventHubSimulator/master/Image1.PNG)

## Here are some of the highlights questions:

1. What to use Service Bus or Event Hub?
2. Does the message/Data or file size impacts the performance?
3. Service Bus Q or Topics & Subscriptions?
4. Service Bus with multiple Subscription or dual Service Bus?
5. Service Bus API: Send or AsyncSend?
6. How do I monitor my solution?
7. After finding the best performance, how does my system looks like?
8. What should be my Paas products Tier?
9. Etc....

![alt tag](https://raw.githubusercontent.com/zivshtaeinberg/AzureServicebusAndEventHubSimulator/master/Image2.PNG)

![alt tag](https://raw.githubusercontent.com/zivshtaeinberg/AzureServicebusAndEventHubSimulator/master/Image3.PNG)


This solution was developed following our integration and the impact we had with real customers.
The solution Uses & Monitor Azure Paas products: Service Bus Q & Topics, Event Hub, Stream Analytics and APIM.
Monitored by Application Insights & Other Log Capabilities.
Scaling UP & Down VM'S with the Application Tool or Triggered by timer.

The Tool is based on WinForm Application.

You can clone this repo and create your own custom logic, just create a new class library and implement [ILoadWatcher](https://github.com/guybartal/AzureVmssCustomAutoScale/blob/master/vmssAutoScale.Interfaces/ILoadWatcher.cs) interface.


## Requirements: Please select by you Cloud Solution Architect dilemma
* Service Bus Q.
* Service Bus Topics & Subscriptions.
* Event Hub & Stream Analytics.
* APIM.
* AAD - If security is required.
* Azure Function App.
* VMSS.

* [Register](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-app-registration) a new application in Azure Active Directory (need to be global admin in order to do that)
* Create application Key, save this secret, you will need that later for deployment
* Give the application "Owner" role on VMSS
* Install [Visual Studio Tools for Azure Functions](https://blogs.msdn.microsoft.com/webdev/2016/12/01/visual-studio-tools-for-azure-functions/)
* Create SQL Database and/or ServiceBus Topics or and Q.
* Create Application Insights.

## Scaling Logic Parameters
* These parameters decides if input data is X & Y will the scaling will be up with Z VM'S or down.
* Please decide where to store the scaling logic parameters to be store, the options are Database or File.

## Deployment
* Open solution in visual studio 2015 update 3
* Build solution
* Publish Azure Function project named "vmssAutoScale" into your Azure Function App
* Set Azure Function App settings:
```XML
  <appSettings>
    <add key="MaxScale" value="maximum server capacity limit"/>
    <add key="MinScale" value="minimum server capacity limit"/>
    <add key="MaxThreshold" value="maximum threshold for auto scale, above this value autoscaler will add one server to vmss"/>
    <add key="MinThreshold" value="minimum threshold for auto scale, below this value autoscaler will remove one server to vmss"/>
    <add key="SQLConnectionString" value="sql server connection string which holds logic for autoscale"/>
    <add key="ClientId" value="application key in azure active directory"/>
    <add key="ClientSecret" value="application secret in azure active directory"/>
    <add key="TenantId" value="active directory id"/>
    <add key="SubscriptionId" value="azure subscription id which holds vmss"/>
    <add key="ResourceGroup" value="vmss resource group"/>
    <add key="VmssName" value="vmss name"/>
    <add key="AzureArmApiBaseUrl" value="https://management.azure.com/"/>
    <add key="VmssApiVersion" value="2016-03-30"/>
  </appSettings>
 ```
