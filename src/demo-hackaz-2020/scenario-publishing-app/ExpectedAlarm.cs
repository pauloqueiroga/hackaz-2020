using Position4All.DemoPublishingApp.Messages;
using System;

namespace Position4All.DemoPublishingApp
{
    internal class ExpectedAlarm
    {
        public string AlarmRegion { get; set; }

        public string AlarmType { get; set; }

        public string AlarmId { get; set; }

        public State CurrentState { get; set; }

        internal string ToCsv()
        {
            return $"{AlarmType},{AlarmRegion},1,{AlarmId},{CurrentState.ToCsv()}";
        }
    }
}