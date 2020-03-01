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
    public class GetJOGCommonParamsResponse : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "dobot_msgs/GetJOGCommonParams";

        public int result;
        public float velocityRatio;
        public float accelerationRatio;

        public GetJOGCommonParamsResponse()
        {
            this.result = 0;
            this.velocityRatio = 0.0f;
            this.accelerationRatio = 0.0f;
        }

        public GetJOGCommonParamsResponse(int result, float velocityRatio, float accelerationRatio)
        {
            this.result = result;
            this.velocityRatio = velocityRatio;
            this.accelerationRatio = accelerationRatio;
        }
    }
}