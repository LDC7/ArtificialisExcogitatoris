namespace ConsoleApp1
{
    using SentenceFinisher;
    using System;

  class Program
  {
    static void Main(string[] args)
    {
      /*DataBaseAccesser.AddExcludedWord("@pat");*/

      
      DataBaseAccesser.AddExcludedWord("@here");
      DataBaseAccesser.AddExcludedWord("@everyone");
      

    }
  }
}
