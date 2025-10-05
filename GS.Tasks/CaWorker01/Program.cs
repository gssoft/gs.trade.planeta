// See https://aka.ms/new-console-template for more information
// https://learn.microsoft.com/ru-ru/dotnet/api/system.threading.cancellationtoken?view=net-7.0

using CaWorker01;
using GS.ConsoleAS;
using System.Threading;

Console.WriteLine("Hello, World!");

CancellationTokenSource source = new CancellationTokenSource();
CancellationToken token = source.Token;

// var token = new CancellationTokenSource().Token;

//Task.Factory.StartNew(() =>
//{
//    // await Task.Delay(5000);
//    ConsoleSync.WriteLineT("Main => Running ...");
//    Task.Delay(5000);
//    source.Cancel();
//});
var thread0 = Thread.CurrentThread;
ConsoleSync.WriteLineT($"Main => Thread:{thread0.ManagedThreadId} Running ...");

Action lambda1 = async () =>
{
    var thread = Thread.CurrentThread;
    ConsoleSync.WriteLineT($"Task1 => Thread:{thread.ManagedThreadId} Running ...");
    // Task.Delay(5000);    //.ConfigureAwait(false);
    await Task.Delay(5000).ConfigureAwait(false);
    // Thread.Sleep(5000);
    source.Cancel();
    ConsoleSync.WriteLineT($"Task1 => Thread:{thread.ManagedThreadId} Completing ...");
};
Action lambda2 = async () =>
{
    var thread = Thread.CurrentThread;
    ConsoleSync.WriteLineT($"Task2 => Thread:{thread.ManagedThreadId} Running ...");
    // Task.Delay(5000);    //.ConfigureAwait(false);
    await Task.Delay(5000).ConfigureAwait(false);
    // Thread.Sleep(5000);
    source.Cancel();
    ConsoleSync.WriteLineT($"Task2 => Thread:{thread.ManagedThreadId} Completing ...");
};
var task1 = new Task(() =>
{
    var thread = Thread.CurrentThread; 
    ConsoleSync.WriteLineT($"Task1 => Thread:{thread.ManagedThreadId} Running ...");
    // Task.Delay(5000);    //.ConfigureAwait(false);
    Thread.Sleep(5000);
    source.Cancel();
    ConsoleSync.WriteLineT($"Task1 => Thread:{thread.ManagedThreadId} Completing ...");
});
// task1.Start();

var task2 = new Task(() => 
{
    var thread = Thread.CurrentThread;
    ConsoleSync.WriteLineT($"Task2 => Thread:{thread.ManagedThreadId} Running ...");
    // Task.Delay(5000);   //.ConfigureAwait(false);
    // Task.Delay(5000);
    Thread.Sleep(5000);
    source.Cancel();
    ConsoleSync.WriteLineT($"Task2 => Thread:{thread.ManagedThreadId} Completing ...");
});
//task2.Start();
var task3 = new Task(lambda1);
var task4 = new Task(lambda2);

task1.Start();
task2.Start();

//task3.Start();
//task4.Start();

//await Task.Run(() =>
//    {
//        var thread = Thread.CurrentThread;
//        ConsoleSync.WriteLineT($"Task1 => Thread:{thread.ManagedThreadId} Running ...");
//// Task.Delay(5000);   //.ConfigureAwait(false);
//        Task.Delay(5000);
//        Thread.Sleep(5000);
//        source.Cancel();
//        ConsoleSync.WriteLineT($"Task1 => Thread:{thread.ManagedThreadId} Completing ...");
//    }
//);
//await Task.Run(() =>
//{
//    var thread = Thread.CurrentThread;
//    ConsoleSync.WriteLineT($"Task2 => Thread:{thread.ManagedThreadId} Running ...");
//    // Task.Delay(5000);   //.ConfigureAwait(false);
//    Task.Delay(5000);
//    Thread.Sleep(5000);
//    source.Cancel();
//    ConsoleSync.WriteLineT($"Task2 => Thread:{thread.ManagedThreadId} Completing ...");
//});

var w = new Worker();
await Worker.ExecuteAsync(token);

ConsoleSync.WriteLineT($"Main => Thread:{Thread.CurrentThread.ManagedThreadId} Complete...");


//await Task.Factory.StartNew( () =>
//{
//    // await Task.Delay(5000);
//    ConsoleSync.WriteLineT("Main => Running ...");
//    Task.Delay(5000);
//    source.Cancel();
//});

Console.ReadLine();




