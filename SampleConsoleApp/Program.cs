﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Gma.DataStructures.StringSearch;

namespace Gma.DataStructures.StringSearch.SampleConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var trie = new SuffixTrie<int>(3);
            var trie = new UkkonenTrie<int>(3);
            //You can replace it with other trie data structures too 
            //ITrie<int> trie = new Trie<int>();
            //ITrie<int> trie = new PatriciaSuffixTrie<int>(3);

            
            try
            {
                //Build-up
                BuildUp("sample.txt", trie);
                //Look-up
                LookUp("over", trie);
                LookUp("porta", trie);
                LookUp("supercalifragilisticexpialidocious", trie);

                var serialized = Serialize(trie);
                Console.WriteLine($"Serialized length: {serialized.Length}");

                var deserializedObj = Deserialize<UkkonenTrie<int>>(serialized);
                LookUp("over", deserializedObj);
                LookUp("porta", deserializedObj);
                LookUp("supercalifragilisticexpialidocious", deserializedObj);
            }
            catch (IOException ioException) { Console.WriteLine("Error: {0}", ioException.Message);}
            catch (UnauthorizedAccessException unauthorizedAccessException) { Console.WriteLine("Error: {0}", unauthorizedAccessException.Message);}

            Console.WriteLine("-------------Press any key to quit--------------");
            Console.ReadKey();
        }

        public static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType(), null, 0x7FFF, false, true, null);
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static T Deserialize<T>(string rawXml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(rawXml)))
            {
                DataContractSerializer formatter0 =
                    new DataContractSerializer(typeof(T));
                return (T)formatter0.ReadObject(reader);
            }
        }

        private static void BuildUp(string fileName, ITrie<int> trie)
        {
            IEnumerable<WordAndLine> allWordsInFile = GetWordsFromFile(fileName);
            foreach (WordAndLine wordAndLine in allWordsInFile)
            {
                trie.Add(wordAndLine.Word, wordAndLine.Line);
            }
        }

        private static void LookUp(string searchString, ITrie<int> trie)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Look-up for string '{0}'", searchString);
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            int[] result = trie.Retrieve(searchString).ToArray();
            stopWatch.Stop();

            string matchesText = String.Join(",", result);
            int matchesCount = result.Count();

            if (matchesCount == 0)
            {
                Console.WriteLine("No matches found.\tTime: {0}", stopWatch.Elapsed);
            }
            else
            {
                Console.WriteLine(" {0} matches found. \tTime: {1}\tLines: {2}", matchesCount, stopWatch.Elapsed,
                    matchesText);
            }
        }


        private static IEnumerable<WordAndLine> GetWordsFromFile(string file)
        {
            using (StreamReader reader = File.OpenText(file))
            {
                Console.WriteLine("Processing file {0} ...", file);
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                int lineNo = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lineNo++;
                    IEnumerable<string> words = GetWordsFromLine(line);
                    foreach (string word in words)
                    {
                        yield return new WordAndLine {Line = lineNo, Word = word};
                    }
                }
                stopWatch.Stop();
                Console.WriteLine("Lines:{0}\tTime:{1}", lineNo, stopWatch.Elapsed);
            }
        }

        private static IEnumerable<string> GetWordsFromLine(string line)
        {
            var word = new StringBuilder();
            foreach (char ch in line)
            {
                if (char.IsLetter(ch))
                {
                    word.Append(ch);
                }
                else
                {
                    if (word.Length == 0) continue;
                    yield return word.ToString();
                    word.Clear();
                }
            }
        }

        private struct WordAndLine
        {
            public int Line;
            public string Word;
        }
    }
}