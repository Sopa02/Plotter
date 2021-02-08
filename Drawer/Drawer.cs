using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotter
{
    class Drawer
    {
        /*  Minta az állapot mentéséhez:
            0;0;F
            2110;1485;F
            2300;1200;T
            2310;1195;T
        */
        List<String> log = new List<string>();
        //A fej poziciója mm-ben adott és a fej legkisebb mozgása 1/10mm
        int posX;
        int posY;
        //Rajzterület határai 1/10mm-ben adva, feltételezve (0,0) a minimum
        int maxX;
        int maxY;
        //Toll állapota, true - lent van, false - fent
        bool PenState;
        public Drawer(int maxX, int maxY)
        {
            this.maxX = maxX;
            this.maxY = maxY;
        }
        public int MaxX { get => maxX; }
        public int MaxY { get => maxY; }
        public void MoveTo(int newX, int newY)
        {
            if (this.PosIsValid(newX, newY))
            {
                this.posX = newX;
                this.posY = newY;
                StateAddToLog();
            }
        }
        private void StateAddToLog()
        {
            log.Add($"{this.posX};{this.posY};{(this.PenState ? "T" : "F")}");
        }
        public void PickUp()
        {
            PenState = false;
        }
        public void PutDown()
        {
            PenState = true;
        }
        public void GoHome()
        {
            posX = 0;
            posY = 0;
        }
        public void MoveRel(int relX, int relY)
        {
            int newX = posX + relX;
            int newY = posY + relY;

            if (this.PosIsValid(newX,newY))
            {
                posX = newX;
                posY = newY;
                StateAddToLog();
            }


        }
        public bool PosIsValid(int newX, int newY)
        {
            if (newX < 0 || newX > maxX || newY < 0 || newY > maxY)
            {
                return false;
                throw new Exception($"A kapott ({newX},{newY}) koordináták nem " +
                    $"érvényesek az adott rajzterületen ({maxX},{maxY}) !");
            }
            else
            {
                return true;
            }
        }
        public static void SaveToCSV(string fileName, Drawer draw)
        {
            File.WriteAllLines(fileName, draw.log);
        }

        public void WorkFromCSV(string filename)
        {
            string[] sorok = File.ReadAllLines(filename);
            for (int i = 0; i < sorok.Length; i++)
            {
                string[] seged = sorok[i].Split(';');
                int x = int.Parse(seged[0]);
                int y = int.Parse(seged[1]);
                string allapot = seged[2];

                this.MoveTo(x, y);
                if (allapot == "T")
                {
                    this.PutDown();
                }
                else { this.PickUp(); }
            }
        }
    }
}
