using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODAF.Data.Enums
{
    [Flags]
    public enum UserRole : int
    {
        Member = 0,
        Moderator,
        Administrator
    };

    public enum UserAccess : int
    {
        Normal = 0,
        Pending = 1, 
        Deleted = 2,
        Banned = 3
    }
}
