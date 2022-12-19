using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("带优先级参数的日志节点")]
    [TaskIcon("{SkinColor}LogIcon.png")]
    public class PriorityLog : Action
    {
        [Tooltip("Text to output to the log")]
        public SharedString text;
        [Tooltip("Is this text an error?")]
        public SharedBool logError;
        [Tooltip("Should the time be included in the log message?")]
        public SharedBool logTime;

        [Tooltip("优先级")]
        public SharedFloat priority;

        /// <summary>
        /// 重写获取优先级的方法
        /// </summary>
        /// <returns></returns>
        public override float GetPriority()
        {
            return priority.Value;
        }
        public override TaskStatus OnUpdate()
        {
            // Log the text and return success
            if (logError.Value) {
                Debug.LogError(logTime.Value ? string.Format("{0}: {1}", Time.time, text) : text);
            } else {
                Debug.Log(logTime.Value ? string.Format("{0}: {1}",Time.time, text) : text);
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            text = "";
            logError = false;
            logTime = false;
        }
    }
}