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
    public class GetInfraredSensorResponse : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "dobot_msgs/GetInfraredSensor";

        public int result;
        public byte value;

        public GetInfraredSensorResponse()
        {
            this.result = 0;
            this.value = 0;
        }

        public GetInfraredSensorResponse(int result, byte value)
        {
            this.result = result;
            this.value = value;
        }
    }
}
