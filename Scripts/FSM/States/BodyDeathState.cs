using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI.FSM
{
    public class BodyDeathState: FSMState
    {
        protected override void init()
        {
            this.StateID = FSMStateID.BodyDeath;
        }

        public override void OnStateEnter(FSMBase fsm)
        {
            base.OnStateEnter(fsm);
            //play dead animation
        }
    }
}