using System;

namespace FlexyChains
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Manipulation manipulation = new Manipulation();
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            
        }
       
    }


}