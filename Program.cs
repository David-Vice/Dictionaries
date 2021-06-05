using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CSharpExam
{
    public class SerializeableKeyValue<T1, T2>
    {
        public T1 Key { get; set; }
        public T2 Value { get; set; }
    }
    static class _Dictionaries
    {
        delegate void DictionariesHandler();

        static event DictionariesHandler Serialization; // Serialization Event

        public static List<_Dictionary> Dictionaries { get; set; }

        static _Dictionaries()
        {
            Dictionaries = new List<_Dictionary>();
        } // ctor

        private static void Serial()
        {
            foreach (var item in Dictionaries)
            {
                item.ToSeria();
            }
        } // Event function
        private static void DeSerial()
        {
            foreach (var item in Dictionaries)
            {
                item.FromSeria();
            }
        } // Event function

        public static void AddDictionary()
        {
            Dictionaries.Add(new _Dictionary(true));
        } // Adds new dictionary
        public static void RemoveDictionary()
        {
            if (Dictionaries.Count == 0)
            {
                Console.WriteLine("No dictionaries yet!");
            }
            else
            {
                PrintDictionaries();
                Console.Write("Choose dictionary you want to delete: ");
                int ind = int.Parse(Console.ReadLine()) - 1;
                _Dictionary todel = Dictionaries[ind];
                Dictionaries.Remove(todel);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{todel.GetDType()} dictionary is successfully removed!");
                Console.ResetColor();
            }
        } // Removes dictionary
        public static bool PrintDictionaries()
        {
            if (Dictionaries.Count == 0)
            {
                Console.WriteLine("No dictionaries yet!");
                return false;
            }
            else
            {
                for (int i = 0; i < Dictionaries.Count; i++)
                {
                    Console.WriteLine($"{i + 1} {Dictionaries[i].GetDType()}");
                }
                return true;
            }
        } // Prints all dictionaries
        public static void ClearAll()
        {
            Dictionaries.Clear();
            Console.WriteLine("All dictionaries are empty now!");
        } // Delete all dictionaries
        public static void SaveToXml()
        {
            Serialization = Serial;
            Serialization.Invoke();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<_Dictionary>));
            try
            {
                using (Stream stream = File.Create("Dictionaries.xml"))
                {
                    xmlSerializer.Serialize(stream, Dictionaries);
                }
            }
            catch (Exception)
            {
                throw;
            }
        } // Saves data to xml file
        public static void GetFromXml()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<_Dictionary>));
            try
            {
                Dictionaries = null;
                using (Stream stream = File.OpenRead("Dictionaries.xml"))
                {
                    Dictionaries = (List<_Dictionary>)xmlSerializer.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                throw;
            }
            Serialization = DeSerial;
            Serialization.Invoke();
        } // Gets data from xml file
    }
    public class _Dictionary
    {
        public SerializeableKeyValue<string, List<string>>[] Seria;
        public void ToSeria()
        {
            var list = new List<SerializeableKeyValue<string, List<string>>>();
            if (dictionary != null)
            {
                list.AddRange(dictionary.Keys.Select(key => new SerializeableKeyValue<string, List<string>>() { Key = key, Value = dictionary[key] }));
            }
            Seria = list.ToArray();
        } // Transforms dictionary in order to serialize it
        public void FromSeria()
        {
            dictionary = new Dictionary<string, List<string>>();
            foreach (var item in Seria)
            {
                dictionary.Add(item.Key, item.Value);
            }
        } // Transforms dictionary in order to serialize it

        public string type;
        private Dictionary<string, List<string>> dictionary;

        public _Dictionary()
        {
            dictionary = new Dictionary<string, List<string>>();
        } // ctor
        public _Dictionary(bool input)
        {
            SetType();
            dictionary = new Dictionary<string, List<string>>();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{type} dictionary is successfully added!");
            Console.ResetColor();
        } // ctor input

        #region Main Methods
        public void Translate()
        {
            string[] from_to = type.Split('-');
            Console.WriteLine($"Enter word or sentence(spaces only) you want to translate from {from_to[0]} to {from_to[1]}: ");
            string to_translate = Console.ReadLine();
            string[] words = to_translate.Split(' ');

            string result = "";
            foreach (var item in words)
            {
                if(dictionary.ContainsKey(item.ToLower()))
                {
                    result = result + dictionary[item.ToLower()][0] + " ";
                }
                else
                {
                    result = result + "??? "; 
                }
            }
            Console.CursorTop--;
            Console.CursorLeft = to_translate.Length;
            Console.Write(" ---> ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(result);
            Console.ResetColor();
        } // Translates senctence or word according to the dictionary

        public void AddTranslation()
        {
            string[] from_to = type.Split('-');
            Console.Write($"Enter a word in {from_to[0]}: ");
            string word_in_from = Console.ReadLine();
            ConsoleKeyInfo click;
            Console.Clear();
            do
            {
                Console.WriteLine($"Word in {from_to[0]}: {word_in_from}");
                Console.Write($"Enter a word in {from_to[1]}: ");
                string word_in_to = Console.ReadLine();
                if (dictionary.ContainsKey(word_in_from.ToLower()))
                {
                    dictionary[word_in_from.ToLower()].Add(word_in_to.ToLower());
                }
                else
                {
                    dictionary.Add(word_in_from.ToLower(), new List<string>() { word_in_to.ToLower() });
                }
                Console.WriteLine("Translation is successfully added!");
                Console.WriteLine();
                PrintEl(word_in_from);
                Program.Pause();
                Console.Clear();
                Console.WriteLine($"Do you want to add one more translation to '{word_in_from}'?");
                Console.WriteLine("|1.Yes|\t|2.No|");
                Console.WriteLine("Click...");
                click = Console.ReadKey(true);
                Console.Clear();
            } while (click.Key == ConsoleKey.D1);
        } // Adds new word with translation, if the given word was present, then adds new translation to it

        public void AddToExisting()
        {
            if (dictionary.Count == 0)
            {
                Console.WriteLine("Dictionary is empty!");
            }
            else
            {
                Console.WriteLine("Choose the word to add new translations...");
                for (int i = 0; i < dictionary.Count; i++)
                {
                    Console.WriteLine($"{i + 1} {dictionary.ElementAt(i).Key}");
                }
                Console.Write("Choose: ");
                int ind = int.Parse(Console.ReadLine()) - 1;
                string newtr;
                Console.Clear();
                do
                {
                    Console.WriteLine($"Enter new translation for {dictionary.ElementAt(ind).Key} or 'stop' to exit: ");
                    newtr = Console.ReadLine();
                    if (newtr != "stop")
                    {
                        dictionary.ElementAt(ind).Value.Add(newtr);
                        Console.WriteLine("New translation is successfully added!");
                        Console.WriteLine();
                        PrintEl(dictionary.ElementAt(ind).Key);
                        Program.Pause();
                        Console.Clear();
                    }
                } while (newtr != "stop");
            }
        } // Adds new translations to the words

        public void ChangeTranslation()
        {
            if (dictionary.Count == 0)
            {
                Console.WriteLine("Dictionary is empty!");
            }
            else
            {
                Console.WriteLine("Choose the translation that you would like to change...");
                for (int i = 0; i < dictionary.Count; i++)
                {
                    Console.WriteLine($"{i + 1} {dictionary.ElementAt(i).Key}");
                }
                Console.Write("Choose: ");
                int ind = int.Parse(Console.ReadLine()) - 1;
                Console.Clear();
                Console.WriteLine($"'{dictionary.ElementAt(ind).Key}' is chosen");
                Console.WriteLine("You want to change the word or its translation?: ");
                Console.WriteLine("|1.The word|\t|2.The translation|");
                Console.WriteLine("Click...");
                ConsoleKeyInfo click = Console.ReadKey(true);
                Console.Clear();
                if (click.Key == ConsoleKey.D1)
                {
                    string newkey;
                    do
                    {
                        Console.Write($"Word to replace '{dictionary.ElementAt(ind).Key}': ");
                        newkey = Console.ReadLine();
                        Console.Clear();
                    } while (dictionary.ContainsKey(newkey.ToLower()));
                    List<string> tmp = dictionary.ElementAt(ind).Value;
                    dictionary.Remove(dictionary.ElementAt(ind).Key);
                    dictionary.Add(newkey.ToLower(), tmp);
                    Console.WriteLine("The word was successfully changed!");
                }
                else
                {
                    for (int i = 0; i < dictionary.ElementAt(ind).Value.Count; i++)
                    {
                        Console.WriteLine($"{i + 1} {dictionary.ElementAt(ind).Value[i]}");
                    }
                    Console.Write("Choose: ");
                    int indval = int.Parse(Console.ReadLine()) - 1;
                    Console.Clear();
                    Console.WriteLine($"'{dictionary.ElementAt(ind).Value[indval]}' is chosen");
                    string newval;
                    Console.Write($"Word to replace the given translation: ");
                    newval = Console.ReadLine();
                    dictionary.ElementAt(ind).Value[indval] = newval.ToLower();
                    Console.WriteLine("The translation was successfully changed!");
                }
            }
        } // Changes word or translations

        public void RemoveTranslation()
        {
            if (dictionary.Count == 0)
            {
                Console.WriteLine("Dictionary is empty!");
            }
            else
            {
                Console.WriteLine("Choose the translation that you would like to remove...");
                for (int i = 0; i < dictionary.Count; i++)
                {
                    Console.WriteLine($"{i + 1} {dictionary.ElementAt(i).Key}");
                }
                Console.Write("Choose: ");
                int ind = int.Parse(Console.ReadLine()) - 1;
                Console.Clear();
                Console.WriteLine($"'{dictionary.ElementAt(ind).Key}' is chosen");
                Console.WriteLine("You want to remove the word or its translation?: ");
                Console.WriteLine("|1.The word|\t|2.The translation|");
                Console.WriteLine("Click...");
                ConsoleKeyInfo click = Console.ReadKey(true);
                Console.Clear();
                if (click.Key == ConsoleKey.D1)
                {
                    dictionary.Remove(dictionary.ElementAt(ind).Key);
                    Console.WriteLine("The word was successfully removed!");
                }
                else
                {
                    for (int i = 0; i < dictionary.ElementAt(ind).Value.Count; i++)
                    {
                        Console.WriteLine($"{i + 1} {dictionary.ElementAt(ind).Value[i]}");
                    }
                    if (dictionary.ElementAt(ind).Value.Count == 1)
                    {
                        Console.WriteLine("It is impossible to remove the last translation!");
                    }
                    else
                    {
                        Console.Write("Choose: ");
                        int indval = int.Parse(Console.ReadLine()) - 1;
                        Console.WriteLine($"{dictionary.ElementAt(ind).Value[indval]} is removed");
                        dictionary.ElementAt(ind).Value.RemoveAt(indval);
                    }
                }
            }
        } // Removes word or translations (impossible to remove last translation)

        public void PrintFull()
        {
            if (dictionary.Count == 0)
            {
                Console.WriteLine("Dictionary is empty!");
            }
            else
            {
                foreach (var item in dictionary)
                {
                    PrintEl(item.Key);
                    Console.WriteLine();
                }
            }
        } // Prints all words with translations
        #endregion

        #region Additional
        private void PrintEl(string key)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(key.ToUpper());
            Console.ResetColor();
            foreach (var item in dictionary[key.ToLower()])
            {
                Console.CursorLeft = key.Length;
                Console.Write("  --->  ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(item);
                Console.ResetColor();
            }
        } // Prints one word with translations
        public void SetType()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Enter the language FROM which this dictionary will translate: ");
            Console.ResetColor();
            string from = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Enter the language TO which this dictionary will translate: ");
            Console.ResetColor();
            string to = Console.ReadLine();

            type = from + "-" + to;
        } // no comment
        public string GetDType()
        {
            return type;
        } // no comment
        #endregion

    }
    class Program
    {
        public static void Pause()
        {
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }
        static void Main(string[] args)
        {
            #region System Part
            // Helps you to print Cyrrilic chars
            Console.OutputEncoding = Encoding.GetEncoding("Cyrillic");
            Console.InputEncoding = Encoding.GetEncoding("Cyrillic");
            #endregion

            _Dictionaries.GetFromXml();
            int opt1 = -1;
            do
            {
                Console.WriteLine("1. Show dictionaries");
                Console.WriteLine("2. Add dictionary");
                Console.WriteLine("3. Remove dictionary");
                Console.WriteLine("4. Pick dictionary");
                Console.WriteLine("5. Clear All Dictionaries");
                Console.WriteLine("6. Save and Exit");
                Console.Write("Choose: ");
                opt1 = int.Parse(Console.ReadLine());
                Console.Clear();
                switch (opt1)
                {
                    case 1:
                        _Dictionaries.PrintDictionaries();
                        Pause();
                        Console.Clear();
                        break;
                    case 2:
                        _Dictionaries.AddDictionary();
                        Pause();
                        Console.Clear();
                        break;
                    case 3:
                        _Dictionaries.RemoveDictionary();
                        Pause();
                        Console.Clear();
                        break;
                    case 4:
                        if(_Dictionaries.PrintDictionaries())
                        {
                            Console.Write("Choose dictionary you want to pick: ");
                            int ind = int.Parse(Console.ReadLine()) - 1;
                            int opt2 = -1;
                            Console.Clear();
                            do
                            {
                                Console.WriteLine($"{_Dictionaries.Dictionaries[ind].type} dictionary");
                                Console.WriteLine("1. Translator");
                                Console.WriteLine("2. Add new translation");
                                Console.WriteLine("3. Add translations to the existing word");
                                Console.WriteLine("4. Change translation");
                                Console.WriteLine("5. Remove translation");
                                Console.WriteLine("6. Review full dictionary");
                                Console.WriteLine("7. Back");
                                Console.Write("Choose: ");
                                opt2 = int.Parse(Console.ReadLine());
                                Console.Clear();
                                switch (opt2)
                                {
                                    case 1:
                                        _Dictionaries.Dictionaries[ind].Translate();
                                        Pause();
                                        Console.Clear();
                                        break;
                                    case 2:
                                        _Dictionaries.Dictionaries[ind].AddTranslation();
                                        break;
                                    case 3:
                                        _Dictionaries.Dictionaries[ind].AddToExisting();
                                        Pause();
                                        Console.Clear();
                                        break;
                                    case 4:
                                        _Dictionaries.Dictionaries[ind].ChangeTranslation();
                                        Pause();
                                        Console.Clear();
                                        break;
                                    case 5:
                                        _Dictionaries.Dictionaries[ind].RemoveTranslation();
                                        Pause();
                                        Console.Clear();
                                        break;
                                    case 6:
                                        _Dictionaries.Dictionaries[ind].PrintFull();
                                        Pause();
                                        Console.Clear();
                                        break;
                                    case 7:
                                        Console.Clear();
                                        break;
                                    default:
                                        Console.Clear();
                                        break;
                                }
                            } while (opt2 != 7);
                        }
                        else
                        {
                            Pause();
                            Console.Clear();
                        }
                        break;
                    case 5:
                        _Dictionaries.ClearAll();
                        Pause();
                        Console.Clear();
                        break;
                    case 6:
                        Console.Clear();
                        break;
                    default:
                        Console.Clear();
                        break;
                }
            } while (opt1 != 6);
            _Dictionaries.SaveToXml();
            Console.WriteLine("All changes are saved successfully!");
        }
    }
}
