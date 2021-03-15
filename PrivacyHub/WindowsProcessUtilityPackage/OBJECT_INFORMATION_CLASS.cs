using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyHub.WindowsProcessUtilityPackage
{
    enum OBJECT_INFORMATION_CLASS : int
    {
        ObjectBasicInformation = 0,
        ObjectNameInformation = 1,
        ObjectTypeInformation = 2,
        ObjectAllTypesInformation = 3,
        ObjectHandleInformation = 4
    }
}
