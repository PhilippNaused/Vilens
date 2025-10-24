using System.Reflection;

namespace ClassLibrary2.Renaming;

[Obfuscation(Exclude = false, Feature = "Renaming")]
public class NestingClass
{
    private class Class1
    {
        private class Class2
        { }

        public class Class3
        {
            public int Field1;
            public long Field2;
            public int Method1() => 1;
            public long Method2() => 1;
        }
    }

    public class Class3
    {
        private int Field1;
        private long Field2;
        private int Method1() => 1;
        private long Method2() => 1;

        private class Class4
        {
            private int Field1;
            private long Field2;
            private int Method1() => 1;
            private long Method2() => 1;
        }

        [Obfuscation(Feature = "Renaming", ApplyToMembers = true)]
        private class Class5
        {
            private int Field1;
        }

        [Obfuscation(Feature = "Renaming", ApplyToMembers = false)]
        private class Class6
        {
            private int Field1;
        }
    }
}
