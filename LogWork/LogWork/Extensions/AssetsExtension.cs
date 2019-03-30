using System.IO;
using System.Reflection;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Extensions
{
    [Preserve(AllMembers = true)]
    public class AssetsExtension
    {
        public static Assembly Assembly;
        public static string DefaultResource;

        public static void InitAssetsExtension(string defaultResource, Assembly assembly)
        {
            Assembly = assembly;
            DefaultResource = defaultResource;
        }

        public static Stream GetFile(string name, string resource = null)
        {
            if (name == null)
                return null;

            return Assembly.GetManifestResourceStream($"{Assembly.GetName().Name}.{resource ?? DefaultResource}.{name}");
        }
    }
}