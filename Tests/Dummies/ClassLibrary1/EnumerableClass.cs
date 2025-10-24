#pragma warning disable

namespace ClassLibrary1;
public class EnumerableClass
{
    public IEnumerable<int> GetNumbersPublic() => GetNumbersPrivate();

    private IEnumerable<int> GetNumbersPrivate()
    {
        yield return 1;
        yield return 2;
        for (int i = 0; i < 10; i++)
        {
            yield return 3 + i;
        }
        yield return 13;
        yield return 14;
    }
}
