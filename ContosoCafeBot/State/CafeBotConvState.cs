namespace ContosoCafeBot
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for storing conversation state. 
    /// </summary>
    public class CafeBotConvState : Dictionary<string, object>
    {
        public Dictionary<string, string> convContextKVPair;
        
    }
    
}
