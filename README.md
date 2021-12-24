# AntiDeadLock

A tiny library to help prevent dead-locking multithreaded code

## Features

Locks multiple objects in a way which prevents deadlocking.

### Notice

The goal of this is to prevent deadlocks but this may hinder performance if the locks are held for too long.

## Usage

### Constructor

```csharp
/// <summary>
/// Creates a new AntiDeadLock object and locks the objects specified.
/// </summary>
/// <param name="objects">The list of objects to lock.</param>
public AntiDeadLock(params object[] objects)
```

### Methods

```csharp
/// <summary>
/// Release all locked objects.
/// </summary>
public void ReleaseLocks()
```

### Example

```csharp
// Given two objects
object obj1 = "Resource A";
object obj2 = "Resource B";

// If we use the lock statement it deadlocks because obtaining
// locks in a criss-cross fashion is prone to deadlocks
ThreadPool.QueueUserWorkItem(_ =>
{
    while (true)
        lock (obj2)
            lock (obj1)
                Console.WriteLine("Thread B completed.");
});
while (true)
    lock (obj1)
        lock (obj2)
            Console.WriteLine("Thread A completed.");

// Locking using AntiDeadLock we dont get deadlock because the
// locks are placed in a consistent order
ThreadPool.QueueUserWorkItem(_ =>
{
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
```