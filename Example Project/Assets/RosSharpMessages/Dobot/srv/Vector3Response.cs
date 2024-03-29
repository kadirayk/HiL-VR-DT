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
    public class Vector3Response : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "dobot_msgs/Vector3";

        public double xOut;
        public double yOut;
        public double zOut;

        public Vector3Response()
        {
            this.xOut = 0.0;
            this.yOut = 0.0;
            this.zOut = 0.0;
        }

        public Vector3Response(double xOut, double yOut, double zOut)
        {
            this.xOut = xOut;
            this.yOut = yOut;
            this.zOut = zOut;
        }
    }
}
