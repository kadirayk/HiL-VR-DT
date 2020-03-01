/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */

using Newtonsoft.Json;

namespace RosSharp.RosBridgeClient.MessageTypes.Dobot
{
    public class GetHOMEParamsResponse : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "dobot_msgs/GetHOMEParams";

        public int result;
        public float x;
        public float y;
        public float z;
        public float r;

        public GetHOMEParamsResponse()
        {
            this.result = 0;
            this.x = 0.0f;
            this.y = 0.0f;
            this.z = 0.0f;
            this.r = 0.0f;
        }

        public GetHOMEParamsResponse(int result, float x, float y, float z, float r)
        {
            this.result = result;
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
        }
    }
}
