#pragma warning disable

namespace ClassLibrary1;
public class AsyncClass
{
    public Task<int> GetNumbersPublic() => GetNumbersPrivate();

    private async Task<int> GetNumbersPrivate()
    {
        await Task.Delay(1);
        await Task.Yield();
        return await Task.FromResult(7);
    }
}
