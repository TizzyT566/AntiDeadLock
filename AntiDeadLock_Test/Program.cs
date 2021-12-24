object obj1 = "Resource A";
object obj2 = "Resource B";

bool AntiDeadLockTest = true;

if (AntiDeadLockTest)
{
    ThreadPool.QueueUserWorkItem(_ =>
    {
        Monitor.Enter(obj1);
        while (true)
        {
            AntiDeadLock antiDeadLock = new(obj2, obj1);
            Console.WriteLine("Thread B completed.");
            antiDeadLock.ReleaseLocks();
        }
    });
    while (true)
    {
        AntiDeadLock antiDeadLock = new(obj1, obj2);
        Console.WriteLine("Thread A completed.");
        antiDeadLock.ReleaseLocks();
    }
}
else
{
    ThreadPool.QueueUserWorkItem(_ =>
    {
        while (true)
        {
            lock (obj2)
            {
                lock (obj1)
                {
                    Console.WriteLine("Thread B completed.");
                }
            }
        }
    });
    while (true)
    {
        lock (obj1)
        {
            lock (obj2)
            {
                Console.WriteLine("Thread A completed.");
            }
        }
    }
}