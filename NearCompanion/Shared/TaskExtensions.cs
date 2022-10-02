namespace NearCompanion.Shared
{
    public static class TaskExtensions
    {
        public static async Task WithTimeout(this Task task, int timeoutMs)
        {
            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                return;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        public static async Task<T> WithTimeout<T>(this Task<T> task, int timeoutMs)
        {
            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                return await task;
            }
            else
            {
                throw new TimeoutException();
            }
        }
    }
}
