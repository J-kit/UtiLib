using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Environment
{
    public class WindowsUser
    {
        public static bool IsElevated => HasRole(WindowsBuiltInRole.Administrator);

        public static bool HasRole(WindowsBuiltInRole role)
        {
            bool isElevated;
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);

                isElevated = principal.IsInRole(role);
            }

            return isElevated;
        }
    }
}