using System;

namespace Xamarin.Forms.Extensions
{
    public static class ExceptionExtension
    {
        public static Exception GetRootException(this Exception e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            while (e.InnerException != null)
            {
                e = e.InnerException;
            }

            return e;
        }
    }
}