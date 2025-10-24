using System;
using System.Collections.Generic;
using System.Linq;

public unsafe static int Test(IList<int> list)
{
    return list.Single((Func<int, bool>)global::<Module>.((nint)__ldftn(.)));
}