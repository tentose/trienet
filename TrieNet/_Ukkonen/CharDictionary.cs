﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gma.DataStructures.StringSearch
{
    internal class CharDictionary<T> : Dictionary<char, Edge<T>>
    {
        //TODO Consider using sorted list based implementation to save memory
    }
}