using System.IO;
using System.Collections.Generic;

namespace SampQueryService.QueryResult
{
    public class PlayerList : IQueryDataResult
    {
        private readonly char _opCode = 'd';
        public IEnumerable<PlayerInfo> Players;

        public PlayerList() { }

        public void Deserialize(byte[] data)
        {
            var pList = new List<PlayerInfo>();

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int maxPlayers = reader.ReadInt16();
                    for (int i = 0; i < maxPlayers; i++)
                    {
                        var pInfo = new PlayerInfo();
                        pInfo.ID = reader.ReadByte();
                        int usernameLength = reader.ReadByte();
                        pInfo.UserName = new string(reader.ReadChars(usernameLength));
                        pInfo.Level = reader.ReadInt32();
                        pInfo.ping = reader.ReadInt32();
                        pList.Add(pInfo);
                    }
                }
            }

            Players = pList;
        }

        public char GetOpCode()
        {
            return _opCode;
        }
    }

}
