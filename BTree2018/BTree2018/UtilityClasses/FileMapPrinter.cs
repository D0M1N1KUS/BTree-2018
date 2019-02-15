using System;
using BTree2018.BTreeIOComponents;

namespace BTreeFileReader
{
    public class FileMapPrinter
    {
        public void printFileMap(FileMap map, int columns, char occupied = '1', char unoccupied = '0')
        {
            Console.WriteLine("Map size [{0}]", map.CurrentMapSize);
            var cols = columns > 0 ? columns : 10;
            Console.Write("\t");
            for(var i = 0; i < cols; i++)
            {
                Console.Write("{0} ", i+1);
            }
            Console.Write("\n\n");

            for (var i = 0; i <= map.CurrentMapSize / cols; i++)
            {
                Console.Write("{0}\t", i);
                for (var j = 0; j < 10; j++)
                {
                    if (map.CurrentMapSize > i * cols + j)
                        Console.Write("{0} ", map[i * cols + j] ? occupied : unoccupied);
                    else
                        break;
                }
                Console.Write('\n');
            }
            
            Console.WriteLine("\n\nEnd of file");
        }
    }
}