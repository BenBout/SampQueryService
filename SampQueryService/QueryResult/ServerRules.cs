using System;
using System.Collections.Generic;
using System.IO;

namespace SampQueryService.QueryResult
{
    public class ServerRules : IQueryDataResult
    {
        private readonly char _opCode = 'r';

        public string MapName { get; set; }
        public float Gravity { get; set; }
        public int Weather { get; set; }
        public string WebUrl { get; set; }
        public ServerTime WorldTime { get; set; }

        public bool Deserialize(byte[] data)
        {
            var resultList = new List<Rule>();

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    
                    int ruleCount = reader.ReadInt16();

                    for (int i = 0; i < ruleCount; i++)
                    {
                        var rule = new Rule();
                        int ruleLength = reader.ReadByte(); // name length
                        rule.Name = new string(reader.ReadChars(ruleLength));

                        ruleLength = reader.ReadByte(); // value length
                        rule.Value = new string(reader.ReadChars(ruleLength));
                        resultList.Add(rule);
                    }
                }
            }

            foreach (var rule in resultList)
            {
                switch (rule.Name)
                {
                    case "mapname": MapName = rule.Value;
                        break;
                    case "weather": Weather = int.Parse(rule.Value);
                        break;
                    case "weburl": WebUrl = rule.Value;
                        break;
                    case "worldtime":
                        {
                            var timeString = rule.Value.Split(':');
                            var time = new ServerTime()
                            {
                                Hour = int.Parse(timeString[0]),
                                Minute = int.Parse(timeString[1])
                            };
                            WorldTime = time;
                        }
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        public char GetOpCode()
        {
            return _opCode;
        }
    }
}
