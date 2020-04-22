using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tests
{
    public class FlowControllerTest 
    {
        [Test]
        public void AddedEventIsExecuted()
        {
            var executed = false;

            FlowController.AddEvent(() => executed = true);

            while (!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
            }

            Assert.IsTrue(executed);
        }

        [Test]
        public void CascadingEventsAreExecuted()
        {
            var executed = false;

            Action addEvent = (() => FlowController.AddEvent(() => executed = true));

            FlowController.AddEvent(addEvent);

            while(!FlowController.ReadyForInput)
            {
                FlowController.TriggerNextAction();
            }

            Assert.IsTrue(executed);
        }
        
        [Test]
        public void AddingEventMakesNotReadyForAction()
        {
            var executed = false;

            FlowController.AddEvent(() => executed = true);

            Assert.IsFalse(FlowController.ReadyForInput);

        }
        [Test]
        public void AddingEventDoesNotExecuteAutomatically()
        {
            var executed = false;

            FlowController.AddEvent(() => executed = true);

            Assert.IsFalse(executed);

        }
        [Test]
        public void ExectuingEventMakesReadyForAction()
        {
            var executed = false;

            FlowController.AddEvent(() => executed = true);

            FlowController.TriggerNextAction();

            Assert.IsTrue(FlowController.ReadyForInput);
        }


    }
}