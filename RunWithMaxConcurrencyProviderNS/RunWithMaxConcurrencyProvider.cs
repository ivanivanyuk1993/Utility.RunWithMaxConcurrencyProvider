namespace RunWithMaxConcurrencyProviderNS;

public static class RunWithMaxConcurrencyProvider
{
    public static void RunWithMaxConcurrency(Action action)
    {
        var finishEvent = new CountdownEvent(initialCount: Environment.ProcessorCount);
        var startEvent = new ManualResetEvent(initialState: false);
        var threadCreatedEvent = new CountdownEvent(initialCount: Environment.ProcessorCount);

        for (var threadIndex = 0; threadIndex < Environment.ProcessorCount; threadIndex++)
            new Thread(start: () =>
            {
                threadCreatedEvent.Signal();
                startEvent.WaitOne();

                action();

                finishEvent.Signal();
            })
                .Start();

        threadCreatedEvent.Wait();
        startEvent.Set();
        finishEvent.Wait();
    }
}