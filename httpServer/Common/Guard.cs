using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Common
{
    public static class Guard
    {
        public static void AgainstNull(object value, string name = null)
        {
            if (value == null)
            {
                name ??= "value";

                throw new ArgumentNullException($"{name} cannot be null.");
            }
        }
    }
}
