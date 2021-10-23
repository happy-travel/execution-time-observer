# Execution time observer
Send notification if function executes longer then time

## Usage
```csharp
var value = await TimeObserver.Execute(observedFunc: () => longRunningFunction,
    notifyFunc: notificationFunction,
    notifyAfter: TimeSpan.FromSeconds(1));
```
