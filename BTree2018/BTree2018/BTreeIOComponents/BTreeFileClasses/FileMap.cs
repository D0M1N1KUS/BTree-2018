using System;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents
{
    public class FileMap : IFileBitmap
    {
        public IFileIO FileIO;
        public byte CachedMapPiece => cachedMapPiece;
        public long CurrentMapSize => mapSize;

        private const long FILE_INFO_LENGTH = 8;
        
        private byte cachedMapPiece;
        private bool cacheEmpty = true;
        private bool cachedMapChanged = false;
        private long cachedMapPieceIndex = -1;

        private long mapSize;

        public FileMap(IFileIO fileIO = null)
        {
            if (fileIO != null)
            {
                FileIO = fileIO;
            }
            else
            {
                //TODO: create new FileIO
                
            }
            mapSize = BitConverter.ToInt64(FileIO.GetBytes(0, FILE_INFO_LENGTH), 0);
            if (mapSize != 0) return;
            FileIO.WriteZeros(FILE_INFO_LENGTH, 1);
            FileIO.WriteBytes(BitConverter.GetBytes((long)8), 0);
            mapSize = 8;
            cachedMapPiece = 0;
            cachedMapPieceIndex = 0;
            cacheEmpty = false;
        }

        ~FileMap()
        {
            Flush();
        }
        
        public bool this[long index]
        {
            get
            {
                if (index >= mapSize)
                    throw new ArgumentException("The index (" + index + ") is larger than map size (" + mapSize + ").");
                getNewCachedMapPiece(index);
                return bitIsSet(cachedMapPiece, (int)(7 - index % 8));
            }
            set
            {
                getNewCachedMapPiece(index);
                cachedMapChanged = true;
                setBit(ref cachedMapPiece, (int)(7 - index % 8), value);
            }
        }

        private void getNewCachedMapPiece(long index)
        {
            if (cacheEmpty || index / 8 != cachedMapPieceIndex)
            {
                if (!cacheEmpty && cachedMapChanged) Flush();
                cachedMapPieceIndex = index / 8;
                cacheEmpty = false;
                if (index >= mapSize)
                {
                    increaceMapSize(index);
                    cachedMapPiece = 0;
                    return;
                }
                cachedMapPiece = FileIO.GetByte(cachedMapPieceIndex + FILE_INFO_LENGTH);
            }
        }

        public long GetNextFreeIndex()
        {
            long position = 0;

            var freeBit = getFreeBit(cachedMapPiece);
            if (freeBit != -1) return cachedMapPieceIndex * 8 + freeBit;
            while (true)
            {
                getNewCachedMapPiece(position);
                if (!this[position]) return position;
                position++;
            }
        }

        public void Flush()
        {
            if (!cacheEmpty) FileIO.WriteBytes(new[] {cachedMapPiece}, cachedMapPieceIndex + FILE_INFO_LENGTH);
        }

        private static long getFreeBit(byte mapPiece)
        {
            for (var i = 0; i < 8; i++)
            {
                if (!bitIsSet(mapPiece, 7 - i))
                    return i;
            }

            return -1;
        }

        private static bool bitIsSet(byte b, int i)
        {
            return (b & (1 << i)) != 0;
        }

        private static void setBit(ref byte b, int i, bool set)
        {
            byte setByte = 1;
            setByte <<= i;
            var unsetByte = (byte)~setByte;
            if (set)
                b |= setByte;
            else
                b &= unsetByte;
        }

        private void increaceMapSize(long index)
        {
            if (index >= mapSize)
            {
                var newMapSize = index + 8 - index % 8;
                FileIO.WriteZeros(mapSize, newMapSize);
                FileIO.WriteBytes(BitConverter.GetBytes(newMapSize), 0);
                mapSize = newMapSize;
            }
        }
    }
}