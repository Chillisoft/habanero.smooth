using System.IO;

namespace TestProjectNoDBSpecificProps.BO
{
    public static class BOBroker
    {
        public static string GetClassDefsXml()
        {
            StreamReader classDefStream = new StreamReader(
                typeof(BOBroker).Assembly.GetManifestResourceStream("TestProjectNoDBSpecificProps.BO.ClassDefs.xml"));
            return classDefStream.ReadToEnd();
        }
    }
}