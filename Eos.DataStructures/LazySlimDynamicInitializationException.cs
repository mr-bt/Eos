using System;

namespace Eos.DataStructures
{
    public class LazySlimDynamicInitializationException : Exception
    {
        private const string MessageFormat = "{0} wasn't initialized.";

        public LazySlimDynamicInitializationException(Type innerType, Exception innerException)
            : base(GetMessage(innerType), innerException)
        {
        }

        private static string GetMessage(Type type)
        {
            return string.Format(MessageFormat, type.FullName);
        }
    }
}
