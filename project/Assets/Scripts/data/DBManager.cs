using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tabtoy;
using UnityEngine;
using Newtonsoft.Json;

 
    public class DBManager
    {
        public static IEnumerator LoadConfig()
        {
            var text_json = Resources.Load<TextAsset>(@"DB\Config");
            using (MemoryStream memStream = new MemoryStream())
            {
                BufferedStream bufStream = new BufferedStream(memStream);
                bufStream.Write(text_json.bytes, 0, text_json.bytes.Length);
                bufStream.Position = 0;


                var reader = new tabtoy.DataReader(bufStream);
                var config = new table.Config();
                var result = reader.ReadHeader(config.GetBuildID());
                if (result != FileState.OK)
                {
                    Console.WriteLine("combine file crack!");
                    yield break;
                }

                table.Config.Deserialize(config, reader);
            }
        }
    }

