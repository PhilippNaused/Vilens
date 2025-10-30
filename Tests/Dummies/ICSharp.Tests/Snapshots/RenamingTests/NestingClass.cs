public class NestingClass
{
    private class a
    {
        private class a
        {
        }
        public class b
        {
            public int a;
            public long b;
            public int a()
            {
                return 1;
            }
            public long b()
            {
                return 1L;
            }
        }
    }
    public class Class3
    {
        private class a
        {
            private int m_a;
            private long m_b;
            private int a()
            {
                return 1;
            }
            private long b()
            {
                return 1L;
            }
        }
        private class Class5
        {
            private int Field1;
        }
        private class Class6
        {
            private int a;
        }
        private int m_a;
        private long m_b;
        private int a()
        {
            return 1;
        }
        private long b()
        {
            return 1L;
        }
    }
}