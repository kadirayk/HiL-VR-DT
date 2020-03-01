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
    public class SetEMotorResponse : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "dobot_msgs/SetEMotor";

        public int result;
        public ulong queuedCmdIndex;

        public SetEMotorResponse()
        {
            this.result = 0;
            this.queuedCmdIndex = 0;
        }

        public SetEMotorResponse(int result, ulong queuedCmdIndex)
        {
            this.result = result;
            this.queuedCmdIndex = queuedCmdIndex;
        }
    }
}
