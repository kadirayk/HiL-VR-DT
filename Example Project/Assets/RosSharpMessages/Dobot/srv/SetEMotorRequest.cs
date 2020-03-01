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
    public class SetEMotorRequest : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "dobot_msgs/SetEMotor";

        public byte index;
        public byte isEnabled;
        public float speed;
        public bool isQueued;

        public SetEMotorRequest()
        {
            this.index = 0;
            this.isEnabled = 0;
            this.speed = 0.0f;
            this.isQueued = false;
        }

        public SetEMotorRequest(byte index, byte isEnabled, float speed, bool isQueued)
        {
            this.index = index;
            this.isEnabled = isEnabled;
            this.speed = speed;
            this.isQueued = isQueued;
        }
    }
}
