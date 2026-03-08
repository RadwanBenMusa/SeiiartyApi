

namespace Seiiarty.Services.db
{
    public static class DoSomeThings
    {
        static public string GetFromat(DateTime dateTime) 
        {
            return $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} 00:00:00";
        }
    }
}
