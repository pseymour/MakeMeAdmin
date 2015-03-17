
namespace SinclairCC.MakeMeAdmin
{
    using System;
    //using System.Collections.Generic;
    using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;

    // TODO: How do we know we are being passed a valid SID?

    internal class PrincipalList
    {
        private static System.Collections.Generic.Dictionary<string, DateTime> principals = null;

        private PrincipalList()
        {
        }

        static PrincipalList()
        {
            principals = new System.Collections.Generic.Dictionary<string, DateTime>();

            // Retrieve the stored SID list from the settings.
            string[] storedSIDs = Settings.SIDs;
            if (storedSIDs != null)
            {
                for (int i = 0; i < storedSIDs.Length; i++)
                {
                    PrincipalList.AddSID(storedSIDs[i], DateTime.Now.AddMinutes(Settings.AdminRightsTimeout * -1));
                }
            }
            Settings.SIDs = null;
        }

        public static void AddSID(string sid)
        {
            AddSID(sid, DateTime.Now);
        }

        public static void AddSID(string sid, DateTime timeAdded)
        {
            if (!principals.ContainsKey(sid))
            {
                principals.Add(sid, timeAdded);
            }
        }

        public static void RemoveSID(string sid)
        {
            if (principals.ContainsKey(sid))
            {
                principals.Remove(sid);
            }
        }

        public static string[] GetSIDs()
        {
            return principals.Select(p => p.Key).ToArray<string>();
        }

        public static string[] GetExpiredSIDs()
        {
            return principals.Where(p => p.Value <= DateTime.Now.AddMinutes(-1 * Settings.AdminRightsTimeout)).Select(p => p.Key).ToArray<string>();
        }
    }

}
