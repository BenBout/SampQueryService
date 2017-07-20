﻿using System;
using System.IO;

namespace SampQueryService.QueryResult
{
    public class ServerInfo : IQueryDataResult
    {
        private readonly char _opCode = 'i';
        public bool Password { get; private set; }
        public int Players { get; private set; }
        public int MaxPlayers { get; private set; }
        public string HostName { get; private set; }
        public string GameModeName { get; private set; }
        //public string MapName { get; private set; }
        public string Language { get; private set; }

        public void Deserialize(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int length;

                    Password = Convert.ToBoolean(reader.ReadByte());
                    Players = reader.ReadInt16();
                    MaxPlayers = reader.ReadInt16();

                    length = reader.ReadInt32();
                    HostName = new string(reader.ReadChars(length));

                    length = reader.ReadInt32();
                    GameModeName = new string(reader.ReadChars(length));

                    length = reader.ReadInt32();
                    Language = new string(reader.ReadChars(length));

                    //length = reader.ReadInt32();
                    //MapName = new string(reader.ReadChars(length));
                }
            }
        }

        public char GetOpCode()
        {
            return _opCode;
        }
    }

}
