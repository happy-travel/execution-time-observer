#Execution time observer
Send notification if function executes longer then time

##Usage
```csharp
var value = await ExecutionTimeObserverHelper.Execute(funcToExecute: () => longRunningFunction,
    funcToNotify: notificationFunction,
    notifyAfter: TimeSpan.FromSeconds(1));
```