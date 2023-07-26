using System;
using System.Threading.Tasks;

namespace Src.Locomotion
{
    public delegate void AsyncTaskRunner(Func<Task> action);

    public delegate Task AsyncTaskRunnerAwaitable(Func<Task> action);
}