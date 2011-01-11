using System.IO;

namespace TestProject.BO
{
    public static class BOBroker
    {
        public static string GetClassDefsXml()
        {
            StreamReader classDefStream = new StreamReader(
                typeof(BOBroker).Assembly.GetManifestResourceStream("TestProject.BO.ClassDefs.xml"));
            return classDefStream.ReadToEnd();
        }
    }
}