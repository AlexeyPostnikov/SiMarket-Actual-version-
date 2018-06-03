using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;



namespace SiMarket
{
    class FileWorks
    {
        public void ReadCompaniesList()
        {
            String readLine;
            StreamReader fileRead = new StreamReader("CompaniesList.txt");
            readLine = fileRead.ReadLine();
            while (readLine != null)
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[4]), Int32.Parse(substrings[3]), substrings[1], substrings[2]);
                Market.Current.CompaniesList.List.Add(sp);
                readLine = fileRead.ReadLine();
            }
            fileRead.Close();
        }

        public void CreateFileWithTypesAndLocations()
        {
            string writeType = null;
            string writeLoc = null;
            StreamWriter writer = new StreamWriter("IDKHowToName.txt");
            foreach (SharePacket sp in Market.Current.CompaniesList.List)
            {
                writeType += sp.CompanyType;
                writeLoc += sp.Location;
                writeType += ", ";
                writeLoc += ", ";
            }
            writeLoc = writeLoc.Substring(0, writeLoc.Length - 2);
            writeType = writeType.Substring(0, writeType.Length - 2);
            String[] substringsTypes = writeType.Split(new[] { ", " }, StringSplitOptions.None);
            String[] substringsLoc = writeLoc.Split(new[] { ", " }, StringSplitOptions.None);
            substringsTypes = Market.Current.RemoveDuplicates(substringsTypes);
            substringsLoc = Market.Current.RemoveDuplicates(substringsLoc);
            writeType = String.Join(", ", substringsTypes);
            writeLoc = String.Join(", ", substringsLoc);
            writer.WriteLine(writeType);
            writer.WriteLine(writeLoc);
            writer.Close();
        }


        public void Load()
        {
            Market.Current.BuyList.List.Clear();
            Market.Current.SellList.List.Clear();
            Console.WriteLine("Enter save's name");
            string save = Console.ReadLine();
            save += ".save";

            String readLine;
            StreamReader fileRead = new StreamReader(save);
            readLine = fileRead.ReadLine();
            Market.Current.Difficulty = readLine;
            readLine = fileRead.ReadLine();
            Market.Current.ShowToolTips = bool.Parse(readLine);
            readLine = fileRead.ReadLine();
            Market.Current.DayCounter = Int32.Parse(readLine);
            readLine = fileRead.ReadLine();
            Player.Current.Name = readLine;
            readLine = fileRead.ReadLine();
            Player.Current.Money = decimal.Parse(readLine);
            readLine = fileRead.ReadLine();

            while (readLine != "History")
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[2]), Int32.Parse(substrings[1]));
                sp.Owner = Player.Current.Name;
                Player.Current.Pocket.List.Add(sp);
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != "BuyList")
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[2]), Int32.Parse(substrings[1]));
                sp.Owner = substrings[3];
                Player.Current.History.List.Add(sp);
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != "SellList")
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[3]), Int32.Parse(substrings[2]), Int32.Parse(substrings[1]));
                if (4 < substrings.Count())
                {
                    sp.Owner = substrings[4];
                }
                Market.Current.BuyList.List.Add(sp);
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != "Events")
            {
                SharePacket sp = new SharePacket();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                sp.InitSharePacket(substrings[0], decimal.Parse(substrings[3]), Int32.Parse(substrings[2]), Int32.Parse(substrings[1]));
                if (4 < substrings.Count())
                {
                    sp.Owner = substrings[4];
                }
                Market.Current.SellList.List.Add(sp);
                readLine = fileRead.ReadLine();
            }
            readLine = fileRead.ReadLine();
            while (readLine != null)
            {
                Event evnt = new Event();
                String[] substrings = readLine.Split(new[] { ", " }, StringSplitOptions.None);
                evnt.Location = substrings[0];
                evnt.CompanyType = substrings[1];
                evnt.Influence = substrings[2];
                evnt.Duration = Int32.Parse(substrings[3]);
                if (4 < substrings.Count())
                {
                    evnt.CompanyName = substrings[4];
                }
                Market.Current.CurrentEvents.Add(evnt);
                readLine = fileRead.ReadLine();
            }
            foreach (SharePacket cmp in Market.Current.CompaniesList.List)
            {
                string cmpname = cmp.CompanyName;
                int number = Player.Current.Pocket.FindNumber(cmp.CompanyName);
                if (number != -1)
                {
                    cmp.CurrentQuantity = cmp.Quantity - Market.Current.SellList.GetAmountOfShares(cmpname) - Player.Current.Pocket.List[number].CurrentQuantity;
                }
                else
                {
                    cmp.CurrentQuantity = cmp.Quantity - Market.Current.SellList.GetAmountOfShares(cmpname);
                }
            }


            fileRead.Close();
        }


        public void Save()
        {
            Console.WriteLine("Enter save's name");
            string save = Console.ReadLine();
            save += ".save";
            StreamWriter sw = new StreamWriter(save);
            sw.WriteLine(Market.Current.Difficulty);
            sw.WriteLine(Market.Current.ShowToolTips);
            sw.WriteLine(Market.Current.DayCounter);
            sw.WriteLine(Player.Current.Name);
            sw.WriteLine(Player.Current.Money);
            foreach (SharePacket sp in Player.Current.Pocket.List)
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                sw.WriteLine(towrite);
            }
            sw.WriteLine("History");
            foreach (SharePacket sp in Player.Current.History.List)
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                towrite += ", ";
                towrite += sp.Owner;
                sw.WriteLine(towrite);
            }
            sw.WriteLine("BuyList");
            foreach (SharePacket sp in Market.Current.BuyList.List)
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.Age;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                if (sp.Owner != null)
                {
                    towrite += ", ";
                    towrite += sp.Owner;
                }
                sw.WriteLine(towrite);
            }
            sw.WriteLine("SellList");
            foreach (SharePacket sp in Market.Current.SellList.List)
            {
                string towrite;
                towrite = sp.CompanyName;
                towrite += ", ";
                towrite += sp.Age;
                towrite += ", ";
                towrite += sp.CurrentQuantity;
                towrite += ", ";
                towrite += sp.Price;
                if (sp.Owner != null)
                {
                    towrite += ", ";
                    towrite += sp.Owner;
                }
                sw.WriteLine(towrite);
            }
            sw.WriteLine("Events");
            foreach (Event evnt in Market.Current.CurrentEvents)
            {
                string towrite;
                towrite = evnt.Location;
                towrite += ", ";
                towrite += evnt.CompanyType;
                towrite += ", ";
                towrite += evnt.Influence;
                towrite += ", ";
                towrite += evnt.Duration;
                if (evnt.CompanyName != null)
                {
                    towrite += ", ";
                    towrite += evnt.CompanyName;
                }
                sw.WriteLine(towrite);
            }
            sw.Close();
        }

        public void FindandAddEvent(Event evnt, string findevent)
        {
            string readline;
            StreamReader sr = new StreamReader("Events.txt");
            readline = sr.ReadLine();
            while (readline != findevent)
            {
                readline = sr.ReadLine();
            }
            readline = sr.ReadLine();
            if (readline == "Random")
            {
                Random rand = new Random();
                string type;
                StreamReader reader = new StreamReader("IDKHowToName.txt");
                type = reader.ReadLine();
                String[] types = type.Split(new[] { ", " }, StringSplitOptions.None);
                evnt.CompanyType = types[rand.Next(types.Length)];
                evnt.WhatRandom = "Type";
            }
            else if (readline != "")
            {
                evnt.CompanyType = readline;
            }
            readline = sr.ReadLine();
            if (readline == "Random")
            {
                if (evnt.CompanyType != null)
                {
                    SharePacket cmp = new SharePacket();
                    List<SharePacket> listwType = new List<SharePacket>();
                    int i = 0;
                    foreach (SharePacket packet in Market.Current.CompaniesList.List)
                    {
                        if (Market.Current.CompaniesList.List[i].CompanyType == cmp.CompanyType)
                        {
                            listwType.Add(packet);
                        }
                        i++;
                    }
                    Random rand = new Random();
                    cmp = listwType.ElementAt(rand.Next(listwType.Count()));
                    evnt.Location = cmp.Location;
                }
                else
                {
                    Random rand = new Random();
                    string loc;
                    StreamReader reader = new StreamReader("IDKHowToName.txt");
                    reader.ReadLine();
                    loc = reader.ReadLine();
                    String[] locs = loc.Split(new[] { ", " }, StringSplitOptions.None);
                    evnt.Location = locs[rand.Next(locs.Length)];
                }
                evnt.WhatRandom = "Location";
            }
            else if (readline != "")
            {
                evnt.Location = readline;
            }
            readline = sr.ReadLine();
            if (readline == "Random")
            {
                SharePacket cmp = new SharePacket();
                Random rand = new Random();
                cmp = Market.Current.CompaniesList.List.ElementAt(rand.Next(Market.Current.CompaniesList.List.Count()));
                evnt.CompanyName = cmp.CompanyName;
                evnt.WhatRandom = "Company";
                evnt.CompanyType = cmp.CompanyType;
                evnt.Location = cmp.Location;
            }
            else if (readline != "")
            {
                evnt.CompanyName = readline;
            }
            readline = sr.ReadLine();
            String[] array = readline.Split(new[] { "-" }, StringSplitOptions.None);
            if (array.Count() == 2)
            {
                Random rand = new Random();
                double Min = Double.Parse(array[0], CultureInfo.InvariantCulture);
                double Max = Double.Parse(array[1], CultureInfo.InvariantCulture);
                double Res = rand.NextDouble() * (Max - Min) + Min;
                evnt.Influence += Res;
            }
            else
            {
                evnt.Influence = readline;
            }

            readline = sr.ReadLine();
            if (readline == "forever")
            {
                evnt.Duration = -1;
            }
            else
            {
                evnt.Duration = Int32.Parse(readline);
            }
            readline = sr.ReadLine();
            evnt.Description = readline;
            if (evnt.WhatRandom != null)
            {
                switch (evnt.WhatRandom)
                {
                    case "Type": { evnt.Description += (evnt.CompanyType + "*"); } break;
                    case "Location": { evnt.Description += (evnt.Location + "*"); } break;
                    case "Company": { evnt.Description += (evnt.CompanyName + "*"); } break;
                    default: break;
                }
            }
            readline = sr.ReadLine();
            String[] array1 = readline.Split(new[] { "!" }, StringSplitOptions.None);
            if (array1.Count() > 1)
            {
                switch (evnt.WhatRandom)
                {
                    case "Location": { evnt.EndMessage += (array1[0] + evnt.Location + array1[1]); } break;
                    case "Company": { evnt.EndMessage += (array1[0] + evnt.CompanyName + array1[1]); } break;
                    default: break;
                }
            }
            else
            {
                evnt.EndMessage += (array1[0]);
            }
            Market.Current.CurrentEvents.Add(evnt);
            Console.WriteLine("Event happened:\n {0}", evnt.Description);
            readline = sr.ReadLine();
            if (Market.Current.ShowToolTips == true)
            {
                evnt.Tooltips = readline;
                Console.WriteLine(evnt.Tooltips);
            }
            Console.ReadKey();
            sr.Close();
        }

        public void StartGame()
        {
            Console.WriteLine("Enter your name:");
            Console.Write("$");
            string input = Console.ReadLine();
            if (input.ToLower() == "load")
            {
                Load();
            }
            else
            {
                if (input == "")
                {
                    Player.Current.Name = "Player";
                }
                else
                {
                    Player.Current.Name = input;
                }
                Player.Current.StartingMoney = 50000;
                Console.WriteLine("Enter difficulty (easy/medium/hard, standard - medium)");
                Market.Current.Difficulty = Console.ReadLine();
                if (Market.Current.Difficulty.ToLower() == "easy")
                {
                    Market.Current.ShowToolTips = true;
                    Player.Current.StartingMoney = 100000;
                }
                else if (Market.Current.Difficulty.ToLower() == "hard")
                {
                    Player.Current.StartingMoney = 10000;
                }
            }
        }
    }
}
