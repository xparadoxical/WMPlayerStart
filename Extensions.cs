namespace WMPlayerStart
{
    public static class Extensions
    {
        public static string Connect(this string[] a, char separator, bool isPathArray)
        {
            string s = "";
            foreach (string _s in a)
            {
                s += (isPathArray ? "\"" + _s + "\"" : _s) + " ";
            }
            return s.Remove(s.Length - 1);
        }
        
        public static string Connect(this string[] a, char separator, bool isPathArray, int startIndex, int endIndex)
        {
            string s = "";
            for (int i = startIndex; i <= endIndex; i++)
            {
                s += (isPathArray ? "\"" + a[i] + "\"" : a[i]) + " ";
            }
            return s.Remove(s.Length - 1);
        }
        
        public static bool EqualsToAtLeastOne(this string s, params string[] toCompare)
        {
            bool b = false;
            foreach (string _s in toCompare)
            {
                if (s == _s)
                    b = true;
            }
            return b;
        }
        
        public static bool EqualsToAtLeastOne(this int i, params int[] toCompare)
        {
            bool b = false;
            foreach (int _i in toCompare)
            {
                if (i == _i)
                    b = true;
            }
            return b;
        }
    }
}
