using Baidu.Aip.Face;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiFace
{
    public class FaceHelper
    {
        // 设置APPID/AK/SK
        public static string APP_ID = "10886080";
        public static string API_KEY = "MwxV2GWQhvuwhtsNk0HGwKRj";
        public static string SECRET_KEY = "uzeA6epZqVOXN8nDncP4e0wg2eUmTLIG ";
        public static Face client;

        static FaceHelper()
        {
            client = new Baidu.Aip.Face.Face(API_KEY, SECRET_KEY);
        }

        public static JObject Detect(string file)
        {
            var image = File.ReadAllBytes(file);
            var parms = new Dictionary<string, object>() { { "max_face_num", "2" } };
            // 调用人脸检测，可能会抛出网络等异常，请使用try/catch捕获
            return client.Detect(image, parms);
        }

        public static int Match(string face1, string face2)
        {
            var images = new[]
            {
                File.ReadAllBytes(face1),
                File.ReadAllBytes(face2)
            };
            // 调用人脸比对，可能会抛出网络等异常，请使用try/catch捕获
            JObject result = client.Match(images);
            if (result.Value<int>("result_num") > 0)
            {
                return result["result"].First.Value<int>("score");
            }
            else
            {
                return 0;
            }
        }
        public static Random rm = new Random();
        public static bool AddUserFace(string uid, string userInfo, string groupId, string filename)
        {
            var image = File.ReadAllBytes(filename);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
                    {"action_type", "replace"}
            };
            // 带参数调用人脸注册
            var result = client.UserAdd(uid, userInfo, groupId, image);
            return true;
        }

        public static UserInfo IdentifyDemo(string filename)
        {
            var groupId = "JXGF";
            var image = File.ReadAllBytes(filename);
            //// 调用人脸识别，可能会抛出网络等异常，请使用try/catch捕获
            //var result = client.Identify(groupId, image);
            //Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
                {"ext_fields", "faceliveness"},
                {"user_top_num", 3}
            };
            // 带参数调用人脸识别
            var result = client.Identify(groupId, image, options);
            if (result.Value<int>("result_num") > 0)
            {


                var first = result["result"].First;
                JArray aryScores = JArray.Parse(first["scores"].ToString());
                foreach (var score in aryScores)
                {
                    if (score.Value<double>() >= 80)
                    {
                        UserInfo user = new UserInfo();
                        user.id = first.Value<int>("uid");
                        string userinfo = first.Value<string>("user_info");
                        string[] aryuserinfo = userinfo.Split(',');
                        user.name = aryuserinfo[0];
                        user.tel = aryuserinfo[1];
                        return user;
                    }
                }
                return null;

            }
            else
            {
                return null;
            }

        }


    }
}
